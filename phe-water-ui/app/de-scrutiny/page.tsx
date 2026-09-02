"use client";

import React, { useEffect, useMemo, useState } from "react";

type Applicant = {
  id: number;
  applicationNo: string;
  fullName: string;
  mobileNumber: string;
  email: string;
  aadhaarNumber: string;
  address: string;
  applicationType: string;
  peth: string;
  zone: string;
  propertyNumber: string;
  layoutAddress: string;
  approvedLayoutNumber: string;
  approvedLayoutDate: string | null;
  createdDate: string;
  status: string;
  satBaraPath: string | null;
  approvedLayoutMapPath: string | null;
  geoTagPhotoPath: string | null;
  kmlFilePath: string | null;
  taxNocPath: string | null;
  role: string | null;
  user_code: string | null;
  user_name: string | null;
  scrutiny_status: string | null;
  application_status: string | null;
  entry_date: string | null;
  remark: string | null;
  layoutYesNo: string | null;
  totalPlots: number | null;
  plotsApplicableForThisNoc: number | null;
  amountForPlots: number | null;
  totalEstimateAmount: number | null;
  siteVisitEstimateDocumentPath: string | null;
  siteVisitGeoTagPhotoPath: string | null;
};

type HistoryItem = {
  logId: number;
  applicationNo: string;
  role: string | null;
  userCode: string | null;
  userName: string | null;
  scrutinyStatus: string | null;
  applicationStatus: string | null;
  remark: string | null;
  entryDate: string | null;
};

type Tab = "document" | "status";
type Panel = "details" | "documents" | "history" | null;

const API_URL = "http://localhost:5014/api/DeScrutiny";
const DOCUMENT_API_URL = "http://localhost:5014/api/JEScrutiny";

const CURRENT_ROLE = "Deputy Engineer";
const CURRENT_USER_CODE = "002";

function getStoredUserName(): string {
  if (typeof window === "undefined") return "";

  const candidates = [
    "deUserName",
    "userName",
    "username",
    "loggedInUserName",
    "loggedInUser",
    "employeeName",
    "fullName",
  ];

  for (const key of candidates) {
    const direct = sessionStorage.getItem(key);
    if (direct && direct.trim()) return direct.trim();

    const local = localStorage.getItem(key);
    if (local && local.trim()) return local.trim();
  }

  return "";
}

export default function DeScrutinyPage() {
  const [applications, setApplications] = useState<Applicant[]>([]);
  const [selectedApplication, setSelectedApplication] = useState<Applicant | null>(null);
  const [activePanel, setActivePanel] = useState<Panel>(null);
  const [history, setHistory] = useState<HistoryItem[]>([]);
  const [activeTab, setActiveTab] = useState<Tab>("document");
  const [scrutinyAction, setScrutinyAction] = useState("Accepted");
  const [remark, setRemark] = useState("");
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [historyLoading, setHistoryLoading] = useState(false);
  const [error, setError] = useState("");
  const [historyError, setHistoryError] = useState("");
  const currentUserName = getStoredUserName();

  useEffect(() => {
    fetchApplications();
  }, []);

  async function fetchApplications() {
    try {
      setLoading(true);
      setError("");

      const response = await fetch(API_URL, { cache: "no-store" });
      const text = await response.text();

      if (!response.ok) {
        throw new Error(text || `API Error: ${response.status}`);
      }

      let data: Applicant[];

      try {
        data = JSON.parse(text);
      } catch {
        throw new Error("API ने valid JSON response दिलेला नाही.");
      }

      setApplications(data);
    } catch (err) {
      console.error(err);
      setError(err instanceof Error ? err.message : "Applications fetch करताना error आला.");
    } finally {
      setLoading(false);
    }
  }

  function formatDate(value: string | null) {
    if (!value) return "-";
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) return value;
    return date.toLocaleDateString("en-IN");
  }

  function formatDateTime(value: string | null) {
    if (!value) return "-";
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) return value;
    return date.toLocaleString("en-IN");
  }

  function getStatusClass(status: string | null) {
    const value = (status || "Pending").toLowerCase();

    if (value.includes("accept") || value.includes("verified") || value.includes("completed")) {
      return "approved";
    }

    if (value.includes("reject") || value.includes("send back")) {
      return "rejected";
    }

    return "pending";
  }

  const filteredApplications = useMemo(() => {
    if (activeTab === "document") {
      return applications.filter((app) => {
        const status = (app.application_status || app.status || "").trim().toLowerCase();
        return status === "" || status === "pending" || status === "development charge fixed";
      });
    }

    return applications.filter((app) => (app.application_status || app.status || "").trim().toLowerCase() === "development charge fixed");
  }, [applications, activeTab]);

  function openDetails(app: Applicant) {
    setSelectedApplication(app);
    setActivePanel("details");
    setScrutinyAction("Accepted");
    setRemark(app.remark || "");
  }

  function openDocuments(app: Applicant) {
    setSelectedApplication(app);
    setActivePanel("documents");
  }

  async function openHistory(app: Applicant) {
    setSelectedApplication(app);
    setActivePanel("history");
    setHistory([]);
    setHistoryError("");

    try {
      setHistoryLoading(true);

      const response = await fetch(`${API_URL}/${encodeURIComponent(app.applicationNo)}/history`, {
        cache: "no-store",
      });

      const text = await response.text();

      if (!response.ok) {
        throw new Error(text || `History API Error: ${response.status}`);
      }

      let data: HistoryItem[];

      try {
        data = JSON.parse(text);
      } catch {
        throw new Error("History API ने valid JSON response दिलेला नाही.");
      }

      setHistory(data);
    } catch (err) {
      console.error(err);
      setHistoryError(err instanceof Error ? err.message : "History fetch करताना error आला.");
    } finally {
      setHistoryLoading(false);
    }
  }

  function closePanel() {
    setSelectedApplication(null);
    setActivePanel(null);
    setHistory([]);
    setHistoryError("");
    setRemark("");
  }

  async function saveScrutiny() {
    if (!selectedApplication) return;

    if (!scrutinyAction || scrutinyAction === "Select") {
      alert("Please select a valid Scrutiny Status.");
      return;
    }

    if (!remark.trim()) {
      alert("Remark is required before saving DE verification.");
      return;
    }

    try {
      setSaving(true);

      const response = await fetch(`${API_URL}/${encodeURIComponent(selectedApplication.applicationNo)}/action`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          action: scrutinyAction,
          remark: remark.trim(),
          role: CURRENT_ROLE,
          userCode: CURRENT_USER_CODE,
          userName: currentUserName || "Deputy Engineer",
        }),
      });

      const text = await response.text();
      let result: any = null;

      try {
        result = text ? JSON.parse(text) : null;
      } catch {
        result = null;
      }

      if (!response.ok) {
        throw new Error(result?.message || text || `Update failed: ${response.status}`);
      }

      const updatedApplication: Applicant = {
         ...selectedApplication,
         role: CURRENT_ROLE,
         user_code: CURRENT_USER_CODE,
         user_name: currentUserName || "Deputy Engineer",
         scrutiny_status: scrutinyAction === "Accepted" ? "Accepted" : scrutinyAction === "Rejected" ? "Rejected" : scrutinyAction === "Pending" ? "Pending" : "Send Back to Jr. Engineer",
         application_status: scrutinyAction === "Accepted" ? "Deputy Engineer Verification Completed" : scrutinyAction === "Rejected" ? "Rejected" : scrutinyAction === "Pending" ? "Development Charge Fixed" : "Send Back to Jr. Engineer",
         status: scrutinyAction === "Accepted" ? "Deputy Engineer Verification Completed" : scrutinyAction === "Rejected" ? "Rejected" : scrutinyAction === "Pending" ? "Development Charge Fixed" : "Send Back to Jr. Engineer",
         remark: remark.trim(),
         entry_date: new Date().toISOString(),
      };

      setApplications((previous) => previous.map((item) => item.applicationNo === updatedApplication.applicationNo ? updatedApplication : item));

      alert("Scrutiny details successfully updated.");
      closePanel();
    } catch (err) {
      console.error(err);
      alert(err instanceof Error ? err.message : "Scrutiny update करताना error आला.");
    } finally {
      setSaving(false);
    }
  }

  function getDocumentApiUrl(path: string | null, download = false) {
    if (!path) return null;
    return `${DOCUMENT_API_URL}/document?path=${encodeURIComponent(path)}${download ? "&download=true" : ""}`;
  }

  function DocumentRow({ icon, title, path }: { icon: string; title: string; path: string | null }) {
    const viewUrl = getDocumentApiUrl(path, false);
    const downloadUrl = getDocumentApiUrl(path, true);

    return (
      <div className="document-row">
        <div className="document-icon">{icon}</div>

        <div className="document-info">
          <div className="document-title">{title}</div>

          {path ? (
            <>
              <div className="document-path">{path}</div>

              <div className="document-buttons">
                {viewUrl && (
                  <a href={viewUrl} target="_blank" rel="noopener noreferrer" className="document-view-btn">
                    👁 VIEW
                  </a>
                )}

                {downloadUrl && (
                  <a href={downloadUrl} className="document-download-btn">
                    ⬇ DOWNLOAD
                  </a>
                )}
              </div>
            </>
          ) : (
            <div className="document-not-available">
              Document not uploaded
              <br />
              <span>Not Available</span>
            </div>
          )}
        </div>
      </div>
    );
  }

  return (
    <div className="je-page">
      <header className="je-header-card">
        <div className="je-header-content">
          <div className="je-logo-box">
            <img src="/smc-logo.png.jpg" alt="SMC Logo" className="je-logo" />
          </div>

          <div className="je-heading">
            <div className="je-corporation-name">
              Solapur Municipal Corporation / <span>सोलापूर महानगरपालिका, सोलापूर</span>
            </div>

            <div className="je-project-title">बांधकाम परवानगीता ना-हरकत दाखला मिळणेबाबत</div>

            <div className="je-role">Role: {CURRENT_ROLE}</div>
            <div className="je-welcome">Welcome {currentUserName || "Deputy Engineer"}</div>
          </div>
        </div>
      </header>

      <nav className="je-navigation">
        <button type="button" className={`je-nav-item ${activeTab === "document" ? "active" : ""}`} onClick={() => setActiveTab("document")}>Deputy Engineer Verification</button>
        <button type="button" className={`je-nav-item ${activeTab === "status" ? "active" : ""}`} onClick={() => setActiveTab("status")}>Application Status</button>
      </nav>

      <main className="je-main">
        {!activePanel ? (
          <section className="je-card">
            <div className="je-section-title">
              {activeTab === "document" ? "Deputy Engineer Verification" : "Application Status"}
            </div>

            <div className="je-count">{filteredApplications.length} Applications</div>

            {loading && <div className="message-box">Applications loading...</div>}

            {!loading && error && (
              <div className="error-box">
                <span>{error}</span>
                <button type="button" onClick={fetchApplications}>Retry</button>
              </div>
            )}

            {!loading && !error && filteredApplications.length > 0 && (
              <div className="je-grid-wrapper">
                <table className="je-grid">
                  <thead>
                    <tr>
                      <th className="je-action-head">Action</th>
                      <th>Sr.No</th>
                      <th>Application Date</th>
                      <th>Application No</th>
                      <th>Applicant Name</th>
                      <th>Address</th>
                      <th>Mobile Number</th>
                      <th>Peth</th>
                      <th>Zone</th>
                      <th>Address of Layout</th>
                      <th>Manjur Layout No</th>
                      <th>Layout Date</th>
                      <th>Property No</th>
                      <th>Scrutiny Status</th>
                      <th>Application Status</th>
                    </tr>
                  </thead>

                  <tbody>
                    {filteredApplications.map((app, index) => (
                      <tr key={app.id}>
                        <td className="je-action-cell">
                          <button type="button" className="je-action-btn details" onClick={() => openDetails(app)}>Details</button>
                          <button type="button" className="je-action-btn documents" onClick={() => openDocuments(app)}>Documents</button>
                          <button type="button" className="je-action-btn history" onClick={() => openHistory(app)}>History</button>
                        </td>

                        <td className="je-center je-bold">{index + 1}</td>
                        <td>
                          <div className="je-date-main">{formatDate(app.createdDate)}</div>
                          <div className="je-date-time">{new Date(app.createdDate).toLocaleTimeString("en-IN", { hour: "2-digit", minute: "2-digit" })}</div>
                        </td>
                        <td><span className="je-application-number">{app.applicationNo}</span></td>
                        <td><span className="je-applicant-name">{app.fullName}</span></td>
                        <td>{app.address || "-"}</td>
                        <td className="je-mobile">{app.mobileNumber || "-"}</td>
                        <td>{app.peth || "-"}</td>
                        <td>{app.zone || "-"}</td>
                        <td>{app.layoutAddress || "-"}</td>
                        <td>{app.approvedLayoutNumber || "-"}</td>
                        <td>{formatDate(app.approvedLayoutDate)}</td>
                        <td>{app.propertyNumber || "-"}</td>
                        <td>
                          <span className={`status-badge ${getStatusClass(app.scrutiny_status)}`}>
                            {app.scrutiny_status || "Pending"}
                          </span>
                        </td>
                        <td>
                          <span className={`status-badge ${getStatusClass(app.application_status || app.status)}`}>
                            {app.application_status || app.status || "Pending"}
                          </span>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}

            {!loading && !error && filteredApplications.length === 0 && (
              <div className="message-box">No applications found.</div>
            )}
          </section>
        ) : (
          <section className="details-card">
            {activePanel === "details" && selectedApplication && (
              <>
                <div className="panel-header">
                  Application Details
                  <button type="button" onClick={closePanel} className="close-panel-btn">×</button>
                </div>

                <div className="details-section-title">Application Information</div>
                <div className="details-grid">
                  <DetailField label="Application No" value={selectedApplication.applicationNo} />
                  <DetailField label="Application Date" value={formatDateTime(selectedApplication.createdDate)} />
                  <DetailField label="Applicant Name" value={selectedApplication.fullName} />
                  <DetailField label="Mobile Number" value={selectedApplication.mobileNumber} />
                  <DetailField label="Email" value={selectedApplication.email} />
                  <DetailField label="Aadhaar Number" value={selectedApplication.aadhaarNumber} />
                  <DetailField label="Address" value={selectedApplication.address} />
                  <DetailField label="Application Type" value={selectedApplication.applicationType} />
                </div>

                <div className="details-section-title">Layout Information</div>
                <div className="details-grid">
                  <DetailField label="Peth" value={selectedApplication.peth} />
                  <DetailField label="Zone" value={selectedApplication.zone} />
                  <DetailField label="Property Number" value={selectedApplication.propertyNumber} />
                  <DetailField label="Layout Address" value={selectedApplication.layoutAddress} />
                  <DetailField label="Approved Layout Number" value={selectedApplication.approvedLayoutNumber} />
                  <DetailField label="Approved Layout Date" value={formatDate(selectedApplication.approvedLayoutDate)} />
                </div>

                <div className="details-section-title">Deputy Engineer Verification</div>

                <div className="scrutiny-panel">
                  <div className="scrutiny-grid">
                    <div className="scrutiny-field">
                      <label>Scrutiny Status / पडताळणी</label>
                      <select value={scrutinyAction} onChange={(e) => setScrutinyAction(e.target.value)}>
                        <option value="Select">Select</option>
                        <option value="Accepted">Accepted</option>
                        <option value="Rejected">Rejected</option>
                        <option value="Pending">Pending</option>
                        <option value="Send Back to Jr. Engineer">Send Back to Jr. Engineer</option>
                      </select>
                    </div>

                    <div className="scrutiny-field">
                      <label>Remark / शेरा</label>
                      <textarea value={remark} onChange={(e) => setRemark(e.target.value)} placeholder="Enter remark" rows={4} />
                    </div>
                  </div>

                  <div className="details-footer">
                    <button type="button" className="save-btn" onClick={saveScrutiny} disabled={saving}>
                      {saving ? "Saving..." : "SUBMIT"}
                    </button>

                    <button type="button" className="details-back-btn" onClick={closePanel} disabled={saving}>
                      BACK
                    </button>
                  </div>
                </div>
              </>
            )}

            {activePanel === "documents" && selectedApplication && (
              <>
                <div className="panel-header">
                  Application Documents
                  <button type="button" onClick={closePanel} className="close-panel-btn">×</button>
                </div>

                <div className="panel-application-no">
                  Application No: <strong>{selectedApplication.applicationNo}</strong>
                </div>

                <div className="documents-container">
                  <DocumentRow icon="📜" title="7/12 / Sat Bara" path={selectedApplication.satBaraPath} />
                  <DocumentRow icon="🗺️" title="Approved Layout Map" path={selectedApplication.approvedLayoutMapPath} />
                  <DocumentRow icon="📷" title="Geo Tag Photo" path={selectedApplication.geoTagPhotoPath} />
                  <DocumentRow icon="🌐" title="KML File" path={selectedApplication.kmlFilePath} />
                  <DocumentRow icon="📑" title="Tax NOC" path={selectedApplication.taxNocPath} />
                </div>

                <div className="panel-footer">
                  <button type="button" className="details-back-btn" onClick={closePanel}>CLOSE</button>
                </div>
              </>
            )}

            {activePanel === "history" && selectedApplication && (
              <>
                <div className="panel-header">
                  Application History
                  <button type="button" onClick={closePanel} className="close-panel-btn">×</button>
                </div>

                <div className="panel-application-no">
                  Application No: <strong>{selectedApplication.applicationNo}</strong>
                </div>

                {historyLoading && <div className="message-box">History loading...</div>}
                {!historyLoading && historyError && <div className="error-box">{historyError}</div>}
                {!historyLoading && !historyError && history.length === 0 && <div className="message-box">No history found for this application.</div>}

                {!historyLoading && !historyError && history.length > 0 && (
                  <div className="history-container">
                    {history.map((item) => (
                      <div className="history-card" key={item.logId}>
                        <div className="history-top">
                          <span className="history-role">{item.role || "-"}</span>
                          <span className="history-date">{formatDateTime(item.entryDate)}</span>
                        </div>

                        <div className="history-grid">
                          <div>
                            <strong>User Code</strong>
                            <span>{item.userCode || "-"}</span>
                          </div>

                          <div>
                            <strong>User Name</strong>
                            <span>{item.userName || "-"}</span>
                          </div>

                          <div>
                            <strong>Scrutiny Status</strong>
                            <span className={`status-badge ${getStatusClass(item.scrutinyStatus)}`}>{item.scrutinyStatus || "-"}</span>
                          </div>

                          <div>
                            <strong>Application Status</strong>
                            <span className={`status-badge ${getStatusClass(item.applicationStatus)}`}>{item.applicationStatus || "-"}</span>
                          </div>
                        </div>

                        <div className="history-remark">
                          <strong>Remark</strong>
                          <p>{item.remark || "-"}</p>
                        </div>
                      </div>
                    ))}
                  </div>
                )}

                <div className="panel-footer">
                  <button type="button" className="details-back-btn" onClick={closePanel}>CLOSE</button>
                </div>
              </>
            )}
          </section>
        )}
      </main>

      <style jsx>{`
        .je-page {
          width: 100%;
          max-width: 1250px;
          margin: 24px auto 35px;
          padding: 0 14px;
          color: #333;
          font-family: "Segoe UI", "Noto Sans Devanagari", Mangal, sans-serif;
        }

        .je-header-card {
          width: 100%;
          background: linear-gradient(120deg, #b64d91, #8b397f, #6b286f, #42104f);
          padding: 18px 22px;
          border-radius: 12px;
          box-shadow: 0 5px 16px rgba(0,0,0,.12);
        }

        .je-header-content {
          min-height: 125px;
          display: flex;
          align-items: center;
          justify-content: center;
          gap: 20px;
        }

        .je-logo-box {
          width: 72px;
          height: 72px;
          flex: 0 0 72px;
          border-radius: 50%;
          background: #fff;
          overflow: hidden;
        }

        .je-logo {
          width: 100%;
          height: 100%;
          object-fit: contain;
        }

        .je-heading {
          text-align: center;
        }

        .je-corporation-name {
          color: #fff;
          font-size: 27px;
          font-weight: 800;
        }

        .je-project-title {
          margin-top: 6px;
          color: #fceaf6;
          font-size: 20px;
          font-weight: 800;
        }

        .je-role,
        .je-welcome {
          margin-top: 6px;
          color: #fff;
          font-weight: 700;
        }

        .je-navigation {
          margin-top: 12px;
          display: flex;
          gap: 2px;
          padding: 0 5px;
          background: #fff;
          border-radius: 10px;
          box-shadow: 0 5px 16px rgba(0,0,0,.08);
          overflow-x: auto;
        }

        .je-nav-item {
          border: none;
          border-bottom: 3px solid transparent;
          background: #fff;
          color: #ed168c;
          padding: 13px 15px;
          font-size: 13px;
          font-weight: 800;
          white-space: nowrap;
          cursor: pointer;
        }

        .je-nav-item:hover,
        .je-nav-item.active {
          background: #f7e6f1;
          color: #7d3274;
        }

        .je-nav-item.active {
          border-bottom-color: #ed168c;
        }

        .je-main {
          width: 100%;
          margin-top: 16px;
        }

        .je-card,
        .details-card {
          width: 100%;
          background: #fff;
          border: 1px solid #ececec;
          border-radius: 12px;
          box-shadow: 0 5px 16px rgba(0,0,0,.12);
          overflow: hidden;
        }

        .je-section-title,
        .panel-header {
          text-align: center;
          padding: 13px 18px;
          background: #f7e6f1;
          border-bottom: 1px solid #d9b5ce;
          color: #7d3274;
          font-size: 22px;
          font-weight: 800;
          position: relative;
        }

        .panel-header {
          font-size: 20px;
        }

        .close-panel-btn {
          position: absolute;
          right: 16px;
          top: 8px;
          border: none;
          background: transparent;
          color: #7d3274;
          font-size: 30px;
          cursor: pointer;
        }

        .je-count {
          display: inline-flex;
          margin: 10px 0 10px 14px;
          padding: 7px 14px;
          border: 1px solid #b64d91;
          border-radius: 20px;
          background: #f7e6f1;
          color: #7d3274;
          font-size: 12px;
          font-weight: 800;
        }

        .je-grid-wrapper {
          width: 100%;
          overflow-x: auto;
        }

        .je-grid {
          width: 100%;
          min-width: 1850px;
          border-collapse: separate;
          border-spacing: 0;
        }

        .je-grid th {
          background: linear-gradient(180deg, #f3dce9, #efd3e3);
          color: #4d2851;
          padding: 10px;
          border-right: 1px solid #ddc0d2;
          border-bottom: 2px solid #c995b8;
          text-align: left;
          font-size: 12px;
          font-weight: 800;
          white-space: nowrap;
        }

        .je-grid td {
          padding: 8px 10px;
          border-right: 1px solid #e5dce4;
          border-bottom: 1px solid #e5dce4;
          font-size: 12.5px;
          vertical-align: top;
        }

        .je-grid tbody tr:nth-child(even) {
          background: #fffafd;
        }

        .je-grid tbody tr:hover {
          background: #fff2f9;
        }

        .je-action-head {
          width: 145px;
          min-width: 145px;
        }

        .je-action-cell {
          width: 145px;
          min-width: 145px;
        }

        .je-action-btn {
          width: 100%;
          margin-bottom: 5px;
          min-height: 30px;
          border: none;
          border-radius: 6px;
          padding: 6px;
          color: #fff;
          font-size: 11px;
          font-weight: 800;
          cursor: pointer;
        }

        .je-action-btn.details {
          background: #b64d91;
        }

        .je-action-btn.documents {
          background: #2577b8;
        }

        .je-action-btn.history {
          background: #6b286f;
        }

        .je-center { text-align: center; }
        .je-bold { font-weight: 800; }
        .je-application-number { color: #7d3274; font-weight: 800; white-space: nowrap; }
        .je-applicant-name { font-weight: 800; }
        .je-date-main { font-weight: 800; white-space: nowrap; }
        .je-date-time { margin-top: 3px; color: #7c7180; font-size: 11px; }
        .je-mobile { white-space: nowrap; }

        .status-badge {
          display: inline-flex;
          padding: 6px 10px;
          border-radius: 20px;
          font-size: 11px;
          font-weight: 800;
          white-space: nowrap;
        }

        .status-badge.pending {
          color: #a15c00;
          background: #fff4dc;
          border: 1px solid #ffd98a;
        }

        .status-badge.approved {
          color: #14733c;
          background: #e7f8ee;
          border: 1px solid #a9dfbd;
        }

        .status-badge.rejected {
          color: #a62626;
          background: #ffe9e9;
          border: 1px solid #f1b5b5;
        }

        .details-section-title {
          padding: 13px 20px;
          background: #f7e6f1;
          border-bottom: 1px solid #d9b5ce;
          color: #7d3274;
          font-size: 19px;
          font-weight: 800;
        }

        .details-grid {
          padding: 20px 22px;
          display: grid;
          grid-template-columns: repeat(2, minmax(0,1fr));
          gap: 15px 18px;
        }

        .details-field,
        .scrutiny-field {
          display: flex;
          flex-direction: column;
          gap: 6px;
        }

        .details-field label,
        .scrutiny-field label {
          color: #5c405a;
          font-size: 13px;
          font-weight: 800;
        }

        .details-field input,
        .scrutiny-field select,
        .scrutiny-field textarea {
          width: 100%;
          box-sizing: border-box;
          border: 1px solid #d8c7d5;
          border-radius: 7px;
          padding: 10px 12px;
          background: #fff;
          color: #333;
          font-size: 13px;
        }

        .details-field input {
          background: #faf7fa;
        }

        .scrutiny-panel {
          margin: 20px 22px 22px;
          padding: 18px;
          border: 1px solid #d9b5ce;
          border-radius: 10px;
          background: #fffafd;
        }

        .scrutiny-grid {
          display: grid;
          grid-template-columns: 1fr 1fr;
          gap: 18px;
        }

        .scrutiny-field textarea {
          resize: vertical;
          min-height: 110px;
        }

        .details-footer,
        .panel-footer {
          display: flex;
          justify-content: center;
          gap: 12px;
          padding: 18px;
          border-top: 1px solid #ddd;
        }

        .save-btn,
        .details-back-btn {
          border: none;
          border-radius: 7px;
          padding: 10px 18px;
          font-size: 13px;
          font-weight: 800;
          cursor: pointer;
        }

        .save-btn {
          background: linear-gradient(135deg, #ed168c, #7d3274);
          color: #fff;
        }

        .details-back-btn {
          background: #eee5ec;
          color: #5c405a;
        }

        .save-btn:disabled,
        .details-back-btn:disabled {
          opacity: .6;
          cursor: not-allowed;
        }

        .panel-application-no {
          padding: 15px 20px;
          background: #fffafd;
          color: #5c405a;
          font-size: 14px;
        }

        .documents-container {
          padding: 20px;
        }

        .document-row {
          display: flex;
          gap: 15px;
          padding: 18px;
          margin-bottom: 12px;
          border: 1px solid #e1d4df;
          border-radius: 10px;
          background: #fff;
        }

        .document-icon {
          width: 45px;
          font-size: 28px;
          text-align: center;
        }

        .document-info { flex: 1; }

        .document-title {
          color: #7d3274;
          font-size: 16px;
          font-weight: 800;
        }

        .document-path {
          margin-top: 6px;
          color: #777;
          font-size: 12px;
          word-break: break-all;
        }

        .document-buttons {
          display: flex;
          gap: 8px;
          margin-top: 10px;
        }

        .document-view-btn,
        .document-download-btn {
          display: inline-block;
          padding: 7px 12px;
          border-radius: 6px;
          text-decoration: none;
          font-size: 12px;
          font-weight: 800;
        }

        .document-view-btn { background: #2577b8; color: #fff; }
        .document-download-btn { background: #6b286f; color: #fff; }

        .document-not-available {
          margin-top: 7px;
          color: #a62626;
          font-size: 12px;
          font-weight: 700;
        }

        .document-not-available span {
          color: #888;
        }

        .history-container { padding: 20px; }

        .history-card {
          margin-bottom: 15px;
          padding: 16px;
          border: 1px solid #ddd0da;
          border-radius: 10px;
          background: #fffafd;
        }

        .history-top {
          display: flex;
          justify-content: space-between;
          gap: 15px;
          padding-bottom: 10px;
          border-bottom: 1px solid #eadde7;
        }

        .history-role {
          color: #7d3274;
          font-weight: 800;
        }

        .history-date {
          color: #777;
          font-size: 12px;
        }

        .history-grid {
          display: grid;
          grid-template-columns: repeat(4, 1fr);
          gap: 12px;
          margin-top: 14px;
        }

        .history-grid div {
          display: flex;
          flex-direction: column;
          gap: 5px;
        }

        .history-grid strong,
        .history-remark strong {
          color: #5c405a;
          font-size: 12px;
        }

        .history-grid span {
          font-size: 13px;
        }

        .history-remark {
          margin-top: 15px;
          padding-top: 12px;
          border-top: 1px solid #eadde7;
        }

        .history-remark p {
          margin: 6px 0 0;
          color: #333;
        }

        .message-box {
          margin: 20px;
          padding: 24px;
          text-align: center;
          border-radius: 8px;
          background: #fffafd;
          color: #7d3274;
          font-weight: 700;
        }

        .error-box {
          margin: 20px;
          padding: 15px;
          border-radius: 8px;
          background: #fff0f0;
          color: #a62626;
          font-weight: 700;
        }

        .error-box button {
          margin-left: 15px;
          border: none;
          border-radius: 6px;
          padding: 8px 14px;
          background: #a62626;
          color: #fff;
          font-weight: 800;
          cursor: pointer;
        }

        @media (max-width: 900px) {
          .je-page {
            margin: 12px auto 25px;
            padding: 0 8px;
          }

          .je-header-content {
            flex-direction: column;
          }

          .je-corporation-name {
            font-size: 19px;
          }

          .je-project-title {
            font-size: 16px;
          }

          .details-grid,
          .scrutiny-grid,
          .history-grid {
            grid-template-columns: 1fr;
          }

          .details-footer,
          .panel-footer {
            flex-direction: column;
          }

          .document-row {
            flex-direction: column;
          }
        }
      `}</style>
    </div>
  );
}

function DetailField({ label, value }: { label: string; value: string | number | null | undefined }) {
  return (
    <div className="details-field">
      <label>{label}</label>
      <input type="text" value={value ?? "-"} readOnly />
    </div>
  );
}

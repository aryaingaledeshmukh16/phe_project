"use client";

import { useState, useEffect } from "react";

type Props = {
  applicationNo: string;
  next: () => void;
  back: () => void;
};

interface ApplicantData {
  applicationNo?: string;
  createdDate?: string;
  fullName?: string;
  mobileNumber?: string;
  email?: string;
  aadhaarNumber?: string;
  address?: string;
  applicationType?: string;
  peth?: string;
  zone?: string;
  propertyNumber?: string;
  layoutAddress?: string;
  approvedLayoutNumber?: string;
  approvedLayoutDate?: string;
  totalEstimateAmount?: number;
}

export default function SiteVisitForm({
  applicationNo,
  next,
  back,
}: Props) {
  // State for applicant read-only data
  const [applicantData, setApplicantData] =
    useState<ApplicantData | null>(null);

  const [loadingApplicant, setLoadingApplicant] =
    useState(true);

  const [applicantError, setApplicantError] =
    useState<string | null>(null);

  // Form state
  const [layoutYesNo, setLayoutYesNo] =
    useState("Select");

  const [totalEstimateAmount, setTotalEstimateAmount] =
    useState<number | string>("");

  const [totalPlots, setTotalPlots] =
    useState<number | string>("");

  const [plotsApplicableForThisNoc, setPlotsApplicableForThisNoc] =
    useState<number | string>("");

  const [amountForPlots, setAmountForPlots] =
    useState<number | string>("");

  const [estimateDocument, setEstimateDocument] =
    useState<File | null>(null);

  const [geoTagPhoto, setGeoTagPhoto] =
    useState<File | null>(null);

  const [siteStatus, setSiteStatus] =
    useState("Status");

  const [siteRemark, setSiteRemark] =
    useState("");

  const [loading, setLoading] =
    useState(false);

  // Fetch applicant data on load
  useEffect(() => {
    const fetchApplicant = async () => {
      try {
        console.log(
          "SiteVisitForm: Fetching applicant for",
          applicationNo
        );

        const API_URL =
          `${process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5014"}/api/Applicants/${encodeURIComponent(
            applicationNo
          )}`;

        const response = await fetch(API_URL);

        if (!response.ok) {
          throw new Error(
            `Failed to fetch applicant: ${response.status}`
          );
        }

        const data = await response.json();

        console.log("SiteVisitForm: Applicant data =", data);

        setApplicantData(data);

        if (data.totalEstimateAmount) {
          setTotalEstimateAmount(data.totalEstimateAmount);
        }
      } catch (error) {
        console.error("SiteVisitForm: Fetch error =", error);

        setApplicantError(
          error instanceof Error
            ? error.message
            : "Failed to load applicant data"
        );
      } finally {
        setLoadingApplicant(false);
      }
    };

    fetchApplicant();
  }, [applicationNo]);

  // Calculate amount on plot values change
  const handleCalculate = () => {
    if (layoutYesNo !== "अस्तित्वात नाही") {
      alert("Calculation is only for 'No Pipeline' case.");
      return;
    }

    const totalEstimate = parseFloat(
      totalEstimateAmount.toString()
    );

    const totalPlotsNum = parseInt(
      totalPlots.toString()
    );

    const plotsApplicable = parseInt(
      plotsApplicableForThisNoc.toString()
    );

    // Validation
    if (!totalEstimate || totalEstimate <= 0) {
      alert("Total Estimate Amount must be greater than 0.");
      return;
    }

    if (!totalPlotsNum || totalPlotsNum <= 0) {
      alert("Total Plots must be greater than 0.");
      return;
    }

    if (!plotsApplicable || plotsApplicable <= 0) {
      alert(
        "Plots applicable for this NOC must be greater than 0."
      );
      return;
    }

    if (plotsApplicable > totalPlotsNum) {
      alert(
        "Plots applicable cannot exceed Total Plots."
      );
      return;
    }

    // Calculate
    const estimatePerPlot =
      totalEstimate / totalPlotsNum;

    const estimateForThisNoc =
      estimatePerPlot * plotsApplicable;

    const rounded =
      Math.round(estimateForThisNoc);

    setAmountForPlots(rounded);

    console.log(
      "SiteVisitForm: Calculation =",
      {
        totalEstimate,
        totalPlots: totalPlotsNum,
        plotsApplicable,
        estimatePerPlot,
        estimateForThisNoc,
        rounded
      }
    );
  };

  // Handle submit
  const handleSubmit = async () => {
    // Validation
    if (!applicationNo || applicationNo.trim() === "") {
      alert("Application Number मिळाला नाही.");
      return;
    }

    if (layoutYesNo === "Select") {
      alert(
        "कृपया मंजूर लेआऊट अंतर्गत पाण्याची पाईपलाईन स्थिती निवडा."
      );
      return;
    }

    if (siteStatus === "Status") {
      alert("कृपया Site Status निवडा.");
      return;
    }

    if (!geoTagPhoto) {
      alert("कृपया Geo Tag Photo निवडा.");
      return;
    }

    // Conditional validation for no pipeline
    if (layoutYesNo === "अस्तित्वात नाही") {
      const totalEstimateValue = Number(totalEstimateAmount);
      const totalPlotsValue = Number(totalPlots);
      const plotsApplicableValue = Number(plotsApplicableForThisNoc);
      const calculatedAmountValue = Number(amountForPlots);

      if (!totalEstimateAmount || totalEstimateValue <= 0) {
        alert(
          "कृपया Total Estimate Amount प्रविष्ट करा."
        );
        return;
      }

      if (!totalPlots || totalPlotsValue <= 0) {
        alert("कृपया Total Plots प्रविष्ट करा.");
        return;
      }

      if (
        !plotsApplicableForThisNoc ||
        plotsApplicableValue <= 0
      ) {
        alert(
          "कृपया Plots applicable for this NOC प्रविष्ट करा."
        );
        return;
      }

      if (!amountForPlots || calculatedAmountValue <= 0) {
        alert(
          "कृपया Calculate button वापरून Amount for Plots calculate करा."
        );
        return;
      }

      if (!estimateDocument) {
        alert(
          "कृपया Estimate Document निवडा."
        );
        return;
      }
    }

    try {
      setLoading(true);

      console.log(
        "SiteVisitForm: Submitting for",
        applicationNo
      );

      const formData = new FormData();

      formData.append("layoutYesNo", layoutYesNo);
      formData.append(
        "totalEstimateAmount",
        totalEstimateAmount.toString()
      );
      formData.append("totalPlots", totalPlots.toString());
      formData.append(
        "plotsApplicableForThisNoc",
        plotsApplicableForThisNoc.toString()
      );
      formData.append("amountForPlots", amountForPlots.toString());

      if (estimateDocument) {
        formData.append(
          "siteVisitEstimateDocument",
          estimateDocument
        );
      }

      formData.append(
        "siteVisitGeoTagPhoto",
        geoTagPhoto
      );

      formData.append("status", siteStatus);
      formData.append("remark", siteRemark);

      const API_URL =
        `${process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5014"}/api/Applicants/SiteVisit/${encodeURIComponent(
          applicationNo
        )}`;

      console.log("SiteVisitForm: API URL =", API_URL);

      const response = await fetch(API_URL, {
        method: "POST",
        body: formData,
      });

      console.log(
        "SiteVisitForm: API Status =",
        response.status
      );

      const responseText = await response.text();

      console.log(
        "SiteVisitForm: API Response =",
        responseText
      );

      if (!response.ok) {
        let errorMessage = responseText;

        try {
          const errorJson = JSON.parse(responseText);

          errorMessage =
            errorJson.message ||
            errorJson.error ||
            responseText;
        } catch {
          // Response JSON नसल्यास original text वापरू
        }

        alert(
          `Site Visit Save Failed.\n\nStatus: ${response.status}\n${errorMessage}`
        );

        return;
      }

      let result: any = null;

      if (responseText) {
        try {
          result = JSON.parse(responseText);
        } catch {
          console.warn(
            "SiteVisitForm: API returned non-JSON response."
          );
        }
      }

      console.log(
        "SiteVisitForm: Submit Result =",
        result
      );

      alert("Site Visit Submitted Successfully.");

      // Next page
      next();
    } catch (error) {
      console.error(
        "SiteVisitForm: Submit Error =",
        error
      );

      if (error instanceof TypeError) {
        alert(
          "Server Connection Error.\n\n" +
          "PHE.API चालू आहे का ते check करा.\n" +
          `API URL: ${process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5014"}`
        );
      } else {
        alert("Site Visit save करताना error आला.");
      }
    } finally {
      setLoading(false);
    }
  };

  if (loadingApplicant) {
    return (
      <div className="form-card">
        <div className="form-title">
          <h2>साईट भेट / Site Visit</h2>
        </div>
        <div className="title-border"></div>
        <p>Loading applicant data...</p>
      </div>
    );
  }

  if (applicantError) {
    return (
      <div className="form-card">
        <div className="form-title">
          <h2>साईट भेट / Site Visit</h2>
        </div>
        <div className="title-border"></div>
        <p style={{ color: "red" }}>Error: {applicantError}</p>
      </div>
    );
  }

  return (
    <div className="form-card">
      {/* -------------------------------- */}
      {/* Title */}
      {/* -------------------------------- */}

      <div className="form-title">
        <h2>साईट भेट / Site Visit</h2>
      </div>

      <div className="title-border"></div>

      <h2
        style={{
          padding: "13px 20px",
          background: "#f7e6f1",
          borderBottom: "1px solid #d9b5ce",
          color: "#7d3274",
          fontSize: "19px",
          fontWeight: "800",
          margin: 0,
          width: "100%",
          boxSizing: "border-box",
        }}
      >
        Application Information
      </h2>

      <div className="grid">
        <div>
          <label>Application No</label>
          <input value={applicantData?.applicationNo || applicationNo} readOnly />
        </div>
        <div>
          <label>Application Date</label>
          <input value={applicantData?.createdDate || ""} readOnly />
        </div>
        <div>
          <label>Applicant Name</label>
          <input value={applicantData?.fullName || ""} readOnly />
        </div>
        <div>
          <label>Mobile Number</label>
          <input value={applicantData?.mobileNumber || ""} readOnly />
        </div>
        <div>
          <label>Email</label>
          <input value={applicantData?.email || ""} readOnly />
        </div>
        <div>
          <label>Aadhaar Number</label>
          <input value={applicantData?.aadhaarNumber || ""} readOnly />
        </div>
        <div>
          <label>Address</label>
          <input value={applicantData?.address || ""} readOnly />
        </div>
        <div>
          <label>Application Type</label>
          <input value={applicantData?.applicationType || ""} readOnly />
        </div>
      </div>

      <h2
        style={{
          padding: "13px 20px",
          background: "#f7e6f1",
          borderBottom: "1px solid #d9b5ce",
          color: "#7d3274",
          fontSize: "19px",
          fontWeight: "800",
          margin: 0,
          width: "100%",
          boxSizing: "border-box",
        }}
      >
        Layout Information
      </h2>

      <div className="grid">
        <div>
          <label>Peth</label>
          <input value={applicantData?.peth || ""} readOnly />
        </div>
        <div>
          <label>Zone</label>
          <input value={applicantData?.zone || ""} readOnly />
        </div>
        <div>
          <label>Property Number</label>
          <input value={applicantData?.propertyNumber || ""} readOnly />
        </div>
        <div>
          <label>Layout Address</label>
          <input value={applicantData?.layoutAddress || ""} readOnly />
        </div>
        <div>
          <label>Approved Layout Number</label>
          <input value={applicantData?.approvedLayoutNumber || ""} readOnly />
        </div>
        <div>
          <label>Approved Layout Date</label>
          <input value={applicantData?.approvedLayoutDate || ""} readOnly />
        </div>
      </div>

      <div className="title-border"></div>

      <div className="form-title">
        <h2
          style={{
            padding: "13px 20px",
            background: "#f7e6f1",
            borderBottom: "1px solid #d9b5ce",
            color: "#7d3274",
            fontSize: "19px",
            fontWeight: "800",
            margin: 0,
            width: "100%",
            boxSizing: "border-box",
          }}
        >
          साईट भेट / Site Visit
        </h2>
      </div>

      <div className="grid">
        {/* Layout Pipeline Question */}
        <div style={{ gridColumn: "1 / -1" }}>
          <label>
            मंजूर लेआऊट अंतर्गत पाण्याची पाईपलाईन अस्तित्वात
            आहे का नाही
            <span>*</span>
          </label>
          <select
            value={layoutYesNo}
            onChange={(e) => {
              setLayoutYesNo(e.target.value);
              // Reset related fields
              if (e.target.value === "अस्तित्वात आहे") {
                setTotalPlots("");
                setPlotsApplicableForThisNoc("");
                setAmountForPlots("");
                setEstimateDocument(null);
              }
            }}
          >
            <option value="Select">Select</option>
            <option value="अस्तित्वात आहे">
              अस्तित्वात आहे
            </option>
            <option value="अस्तित्वात नाही">
              अस्तित्वात नाही
            </option>
          </select>
        </div>

        {/* Full width separator */}
        <div
          style={{
            gridColumn: "1 / -1",
            borderTop: "2px solid #ddd",
            margin: "20px 0"
          }}
        ></div>

        {/* Conditional Section: Plot/Estimate (only if No Pipeline) */}
        {layoutYesNo === "अस्तित्वात नाही" && (
          <>
            {/* Total Estimate Amount */}
            <div>
              <label>
                Total Estimate Amount in Rs.
                <span>*</span>
              </label>
              <input
                type="number"
                value={totalEstimateAmount}
                onChange={(e) =>
                  setTotalEstimateAmount(e.target.value)
                }
                placeholder="0"
                min="1"
              />
            </div>

            {/* Total Plots */}
            <div>
              <label>
                Total Plots
                <span>*</span>
              </label>
              <input
                type="number"
                value={totalPlots}
                onChange={(e) =>
                  setTotalPlots(e.target.value)
                }
                placeholder="0"
                min="1"
              />
            </div>

            {/* Plots Applicable */}
            <div>
              <label>
                Plots applicable for this NOC
                <span>*</span>
              </label>
              <input
                type="number"
                value={plotsApplicableForThisNoc}
                onChange={(e) =>
                  setPlotsApplicableForThisNoc(
                    e.target.value
                  )
                }
                placeholder="0"
                min="1"
              />
            </div>

            {/* Calculate Button */}
            <div style={{ gridColumn: "1 / -1" }}>
              <button
                type="button"
                onClick={handleCalculate}
                style={{
                  padding: "10px 20px",
                  backgroundColor: "#007bff",
                  color: "white",
                  border: "none",
                  borderRadius: "5px",
                  cursor: "pointer",
                  fontSize: "14px"
                }}
              >
                Calculate Amount for Plots
              </button>
            </div>

            {/* Amount for Plots Result */}
            <div>
              <label>
                Estimate Amount according to the number
                of plots in Rs.
                <span>*</span>
              </label>
              <input
                type="number"
                value={amountForPlots}
                readOnly
                style={{
                  backgroundColor: "#f5f5f5",
                  cursor: "not-allowed"
                }}
                placeholder="0"
              />
            </div>

            {/* Estimate Document */}
            <div style={{ gridColumn: "1 / -1" }}>
              <label>
                Upload Estimate Document (PDF)
                <span>*</span>
              </label>
              <input
                type="file"
                accept=".pdf"
                onChange={(e) =>
                  setEstimateDocument(
                    e.target.files?.[0] || null
                  )
                }
              />
              {estimateDocument && (
                <small style={{ color: "green" }}>
                  {estimateDocument.name}
                </small>
              )}
            </div>

            {/* Separator */}
            <div
              style={{
                gridColumn: "1 / -1",
                borderTop: "2px solid #ddd",
                margin: "20px 0"
              }}
            ></div>
          </>
        )}

        {/* Geo Tag Photo - Always Required */}
        <div style={{ gridColumn: "1 / -1" }}>
          <label>
            साईटला भेट द्या, जिओ टॅग फोटो अपलोड करा
            <span>*</span>
          </label>
          <input
            type="file"
            accept=".jpg,.jpeg,.png"
            onChange={(e) =>
              setGeoTagPhoto(e.target.files?.[0] || null)
            }
          />
          {geoTagPhoto && (
            <small style={{ color: "green" }}>
              {geoTagPhoto.name}
            </small>
          )}
        </div>

        {/* Separator */}
        <div
          style={{
            gridColumn: "1 / -1",
            borderTop: "2px solid #ddd",
            margin: "20px 0"
          }}
        ></div>

        {/* Site Status */}
        <div style={{ gridColumn: "1 / -1" }}>
          <label>
            Site status / स्टेटस
            <span>*</span>
          </label>
          <select
            value={siteStatus}
            onChange={(e) =>
              setSiteStatus(e.target.value)
            }
          >
            <option value="Status">Status</option>
            <option value="Accept">Accept</option>
            <option value="Reject">Reject</option>
            <option value="Hold">Hold</option>
          </select>
        </div>

        {/* Site Remark */}
        <div style={{ gridColumn: "1 / -1" }}>
          <label>
            Site / Estimate Remark / शेरा
          </label>
          <textarea
            value={siteRemark}
            onChange={(e) =>
              setSiteRemark(e.target.value)
            }
            rows={4}
            placeholder="Enter remarks (optional)"
          />
        </div>
      </div>

      {/* -------------------------------- */}
      {/* Buttons */}
      {/* -------------------------------- */}

      <div className="btn-area">
        <button
          className="reset-btn"
          onClick={back}
          disabled={loading}
        >
          BACK
        </button>

        <button
          className="next-btn"
          onClick={handleSubmit}
          disabled={loading}
        >
          {loading ? "Submitting..." : "SUBMIT"}
        </button>
      </div>
    </div>
  );
}

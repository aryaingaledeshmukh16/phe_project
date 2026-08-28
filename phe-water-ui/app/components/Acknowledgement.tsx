"use client";

import { useEffect, useState } from "react";

type Props = {
  applicationNo: string;
  back: () => void;
};

type AcknowledgementData = {
  applicationNo: string;
  createdDate: string;

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

  taxNocPath: string | null;
  satBaraPath: string | null;
  approvedLayoutMapPath: string | null;
  geoTagPhotoPath: string | null;

  status: string;
};

export default function Acknowledgement({
  applicationNo,
  back,
}: Props) {
  const [data, setData] = useState<AcknowledgementData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchAcknowledgement = async () => {
      try {
        const response = await fetch(
          `http://localhost:5014/api/Applicants/${encodeURIComponent(applicationNo)}`
        );

        if (!response.ok) {
          const error = await response.text();
          console.error(error);

          alert("Acknowledgement details मिळाले नाहीत.");
          return;
        }

        const result = await response.json();

        console.log("Acknowledgement Data:", result);

        setData(result);
      } catch (error) {
        console.error("Acknowledgement Connection Error:", error);

        alert("Server Connection Error.");
      } finally {
        setLoading(false);
      }
    };

    if (applicationNo) {
      fetchAcknowledgement();
    }
  }, [applicationNo]);

  const formatDate = (date: string | null | undefined) => {
    if (!date) return "-";

    const d = new Date(date);

    if (isNaN(d.getTime())) {
      return date;
    }

    return d.toLocaleDateString("en-IN");
  };

  if (loading) {
    return (
      <div className="smc-ack-loading">
        Acknowledgement तयार होत आहे...
      </div>
    );
  }

  if (!data) {
    return (
      <div className="smc-ack-error">
        Acknowledgement details उपलब्ध नाहीत.
        <div style={{ marginTop: 12 }}>
          <button className="smc-btn-outline" onClick={back}>← Back</button>
        </div>
      </div>
    );
  }

  return (
    <div className="smc-ack-root">

      <div className="smc-actions no-print">
        <div>
          <button className="smc-btn-outline" onClick={back}>← Back to Application</button>
        </div>

        <div>
          <button className="smc-btn-outline smc-btn-primary" onClick={() => window.print()}>Print Acknowledgement</button>
        </div>
      </div>

      <div className="smc-receipt" role="document" aria-label="Acknowledgement Receipt">

        <header className="smc-header">
  <div className="smc-emblem" aria-hidden="true">
    <img
      src="/smc-logo.png.jpg"
      alt="Solapur Municipal Corporation Logo"
    />
  </div>

  <div className="smc-titles">
    <div className="marathi">सोलापूर महानगरपालिका</div>
    <div className="english">SOLAPUR MUNICIPAL CORPORATION</div>
    <div className="dept">
      सार्वजनिक आरोग्य अभियांत्रिकी विभाग / PUBLIC HEALTH ENGINEERING DEPARTMENT
    </div>
  </div>
</header>

        <div className="smc-title-block">
          <h1>पिण्याच्या पाण्याच्या पाईपलाईन ना-हरकत प्रमाणपत्र अर्ज</h1>
          <h2>ACKNOWLEDGEMENT RECEIPT</h2>
        </div>

        <div className="smc-summary">
          <div className="sum-item">
            <div className="label">APPLICATION NO.</div>
            <div className="value">{data.applicationNo}</div>
          </div>

          <div className="sum-item">
            <div className="label">APPLICATION DATE</div>
            <div className="value">{formatDate(data.createdDate)}</div>
          </div>

          <div className="sum-item">
            <div className="label">STATUS</div>
            <div className={`value status ${data.status?.toLowerCase()}`}>{data.status}</div>
          </div>
        </div>

        <section className="smc-section">
          <div className="sec-heading">अर्जदाराची माहिती / Applicant Details</div>

          <div className="smc-grid">
            <div className="field">
              <div className="field-label">Applicant Name</div>
              <div className="field-value">{data.fullName || '-'}</div>
            </div>

            <div className="field">
              <div className="field-label">Mobile Number</div>
              <div className="field-value">{data.mobileNumber || '-'}</div>
            </div>

            <div className="field">
              <div className="field-label">Email</div>
              <div className="field-value">{data.email || '-'}</div>
            </div>

            <div className="field">
              <div className="field-label">Aadhaar Number</div>
              <div className="field-value">{data.aadhaarNumber || '-'}</div>
            </div>

            <div className="field full">
              <div className="field-label">Address</div>
              <div className="field-value">{data.address || '-'}</div>
            </div>
          </div>
        </section>

        <section className="smc-section">
          <div className="sec-heading">मंजूर लेआउट माहिती / Approved Layout Details</div>

          <div className="smc-grid">
            <div className="field">
              <div className="field-label">Peth</div>
              <div className="field-value">{data.peth || '-'}</div>
            </div>

            <div className="field">
              <div className="field-label">Zone</div>
              <div className="field-value">{data.zone || '-'}</div>
            </div>

            <div className="field">
              <div className="field-label">Property No.</div>
              <div className="field-value">{data.propertyNumber || '-'}</div>
            </div>

            <div className="field">
              <div className="field-label">Approved Layout No.</div>
              <div className="field-value">{data.approvedLayoutNumber || '-'}</div>
            </div>

            <div className="field full">
              <div className="field-label">Layout Address</div>
              <div className="field-value">{data.layoutAddress || '-'}</div>
            </div>

            <div className="field">
              <div className="field-label">Approved Layout Date</div>
              <div className="field-value">{formatDate(data.approvedLayoutDate)}</div>
            </div>
          </div>
        </section>

        <section className="smc-section">
          <div className="sec-heading">सादर केलेली कागदपत्रे / Documents Submitted</div>

          <div className="smc-docs">
            <div className="doc-item"><span className="doc-check">✓</span> टॅक्स ना-हरकत प्रमाणपत्र</div>
            <div className="doc-item"><span className="doc-check">✓</span> 7/12 उतारा</div>
            <div className="doc-item"><span className="doc-check">✓</span> मंजूर लेआउट नकाशा</div>
            <div className="doc-item"><span className="doc-check">✓</span> Geo Tag Photo</div>
          </div>
        </section>

        <footer className="smc-footer">
          <div className="sig-row">
            <div className="sig-box">
              <div className="sig-line" />
              <div className="sig-label">Applicant Signature</div>
            </div>

            <div className="sig-box">
              <div className="sig-line" />
              <div className="sig-label">Authorized Officer<br/>Public Health Engineering Department</div>
            </div>
          </div>

          <div className="print-note">This is a computer generated acknowledgement receipt and does not require a physical signature.</div>
        </footer>

      </div>

    </div>
  );
}
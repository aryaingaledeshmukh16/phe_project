"use client";

import { useState } from "react";

type LayoutData = {
  applicationType: string;
  peth: string;
  zone: string;
  propertyNumber: string;
  approvedLayoutNumber: string;
  approvedLayoutDate: string;
  layoutAddress: string;
};

type Props = {
  layoutData: LayoutData;
  next: (applicationNo: string) => void;
  back: () => void;
};

export default function DocumentsForm({
  layoutData,
  next,
  back,
}: Props) {
  const [taxNoc, setTaxNoc] = useState<File | null>(null);
  const [satBara, setSatBara] = useState<File | null>(null);
  const [layoutMap, setLayoutMap] = useState<File | null>(null);
  const [geoTag, setGeoTag] = useState<File | null>(null);

  const [loading, setLoading] = useState(false);

  const handleUpload = async () => {
    if (!taxNoc) {
      alert("कृपया Tax NOC निवडा.");
      return;
    }

    if (!satBara) {
      alert("कृपया 7/12 उतारा निवडा.");
      return;
    }

    if (!layoutMap) {
      alert("कृपया मंजूर लेआउट नकाशा निवडा.");
      return;
    }

    if (!geoTag) {
      alert("कृपया Geo Tag Photo निवडा.");
      return;
    }

    try {
      setLoading(true);

      const createPayload = {
        fullName: "",
        mobileNumber: "",
        email: "",
        aadhaarNumber: "",
        address: "",
        applicationType: layoutData.applicationType.trim(),
        peth: layoutData.peth.trim(),
        zone: layoutData.zone.trim(),
        propertyNumber: layoutData.propertyNumber.trim(),
        layoutAddress: layoutData.layoutAddress.trim(),
        approvedLayoutNumber: layoutData.approvedLayoutNumber.trim(),
        approvedLayoutDate: layoutData.approvedLayoutDate || null,
      };

      const createResponse = await fetch(
        "http://localhost:5014/api/Applicants",
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(createPayload),
        }
      );

      const createText = await createResponse.text();

      if (!createResponse.ok) {
        let errorMessage = createText;

        try {
          const errorJson = JSON.parse(createText);
          errorMessage =
            errorJson.message ||
            errorJson.error ||
            createText;
        } catch {
          // ignore parse error
        }

        alert(
          `Application Save Failed.\n\nStatus: ${createResponse.status}\n${errorMessage}`
        );
        return;
      }

      let createResult: any = null;

      if (createText) {
        try {
          createResult = JSON.parse(createText);
        } catch {
          console.warn("Create applicant response was not JSON.");
        }
      }

      const applicantNo = createResult?.applicationNo;

      if (!applicantNo) {
        alert(
          "Application Number was not returned by the server after save."
        );
        return;
      }

      const layoutResponse = await fetch(
        `http://localhost:5014/api/Applicants/Layout/${encodeURIComponent(applicantNo)}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Accept: "application/json",
          },
          body: JSON.stringify({
            applicationType: layoutData.applicationType.trim(),
            peth: layoutData.peth.trim(),
            zone: layoutData.zone.trim(),
            propertyNumber: layoutData.propertyNumber.trim(),
            layoutAddress: layoutData.layoutAddress.trim(),
            approvedLayoutNumber: layoutData.approvedLayoutNumber.trim(),
            approvedLayoutDate: layoutData.approvedLayoutDate,
          }),
        }
      );

      const layoutText = await layoutResponse.text();

      if (!layoutResponse.ok) {
        let errorMessage = layoutText;

        try {
          const errorJson = JSON.parse(layoutText);
          errorMessage =
            errorJson.message ||
            errorJson.error ||
            layoutText;
        } catch {
          // ignore parse error
        }

        alert(
          `Layout Information Save Failed.\n\nStatus: ${layoutResponse.status}\n${errorMessage}`
        );
        return;
      }

      const formData = new FormData();
      formData.append("taxNoc", taxNoc);
      formData.append("satBara", satBara);
      formData.append("layoutMap", layoutMap);
      formData.append("geoTag", geoTag);

      const documentsResponse = await fetch(
        `http://localhost:5014/api/Applicants/Documents/${encodeURIComponent(applicantNo)}`,
        {
          method: "POST",
          body: formData,
        }
      );

      const documentsText = await documentsResponse.text();

      if (!documentsResponse.ok) {
        let errorMessage = documentsText;

        try {
          const errorJson = JSON.parse(documentsText);
          errorMessage =
            errorJson.message ||
            errorJson.error ||
            documentsText;
        } catch {
          // ignore parse error
        }

        alert(
          `Documents Upload Failed.\n\nStatus: ${documentsResponse.status}\n${errorMessage}`
        );
        return;
      }

      alert("Documents Uploaded Successfully.");
      next(applicantNo);
    } catch (error) {
      console.error("Documents Upload Connection Error =", error);

      if (error instanceof TypeError) {
        alert(
          "Server Connection Error.\n\n" +
          "PHE.API चालू आहे का ते check करा.\n" +
          "API URL: http://localhost:5014"
        );
      } else {
        alert("Documents Upload करताना error आला.");
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="form-card">
      <div className="form-title">
        <h2>
          कागदपत्रे / Documents Upload
        </h2>
      </div>

      <div className="title-border"></div>

      <div className="grid">

        {/* Tax NOC */}
        <div>
          <label>
            टॅक्स ना-हरकत प्रमाणपत्र{" "}
            <span>*</span>
          </label>

          <input
            type="file"
            accept=".pdf,.jpg,.jpeg,.png"
            disabled={loading}
            onChange={(e) => {
              setTaxNoc(
                e.target.files?.[0] ?? null
              );
            }}
          />

          {taxNoc && (
            <small>
              Selected: {taxNoc.name}
            </small>
          )}
        </div>

        {/* 7/12 */}
        <div>
          <label>
            7/12 उतारा{" "}
            <span>*</span>
          </label>

          <input
            type="file"
            accept=".pdf,.jpg,.jpeg,.png"
            disabled={loading}
            onChange={(e) => {
              setSatBara(
                e.target.files?.[0] ?? null
              );
            }}
          />

          {satBara && (
            <small>
              Selected: {satBara.name}
            </small>
          )}
        </div>

        {/* Layout Map */}
        <div>
          <label>
            मंजूर लेआउट नकाशा{" "}
            <span>*</span>
          </label>

          <input
            type="file"
            accept=".pdf,.jpg,.jpeg,.png"
            disabled={loading}
            onChange={(e) => {
              setLayoutMap(
                e.target.files?.[0] ?? null
              );
            }}
          />

          {layoutMap && (
            <small>
              Selected: {layoutMap.name}
            </small>
          )}
        </div>

        {/* Geo Tag */}
        <div>
          <label>
            Geo Tag Photo{" "}
            <span>*</span>
          </label>

          <input
            type="file"
            accept=".jpg,.jpeg,.png,.pdf"
            disabled={loading}
            onChange={(e) => {
              setGeoTag(
                e.target.files?.[0] ?? null
              );
            }}
          />

          {geoTag && (
            <small>
              Selected: {geoTag.name}
            </small>
          )}
        </div>

      </div>

      {/* -------------------------------- */}
      {/* Buttons */}
      {/* -------------------------------- */}

      <div className="btn-area">

        <button
          type="button"
          className="reset-btn"
          onClick={back}
          disabled={loading}
        >
          ← Back
        </button>

        <button
          type="button"
          className="next-btn"
          onClick={handleUpload}
          disabled={loading}
        >
          {loading
            ? "Uploading..."
            : "Submit →"}
        </button>

      </div>

    </div>
  );
}
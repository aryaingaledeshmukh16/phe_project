"use client";

import { useState } from "react";

type Props = {
  applicationNo: string;
  next: () => void;
  back: () => void;
};

export default function DocumentsForm({
  applicationNo,
  next,
  back,
}: Props) {
  const [taxNoc, setTaxNoc] = useState<File | null>(null);
  const [satBara, setSatBara] = useState<File | null>(null);
  const [layoutMap, setLayoutMap] = useState<File | null>(null);
  const [geoTag, setGeoTag] = useState<File | null>(null);

  const [loading, setLoading] = useState(false);

  const handleUpload = async () => {
    // --------------------------------
    // Validation
    // --------------------------------

    if (!applicationNo || applicationNo.trim() === "") {
      alert("Application Number मिळाला नाही.");
      return;
    }

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

      console.log(
        "Documents Upload Application No =",
        applicationNo
      );

      // --------------------------------
      // FormData
      // --------------------------------

      const formData = new FormData();

      formData.append("taxNoc", taxNoc);
      formData.append("satBara", satBara);
      formData.append("layoutMap", layoutMap);
      formData.append("geoTag", geoTag);

      // IMPORTANT:
      // Controller class = ApplicantsController
      // Therefore route = api/Applicants
      const API_URL =
        `http://localhost:5014/api/Applicants/Documents/${encodeURIComponent(
          applicationNo
        )}`;

      console.log("Documents API URL =", API_URL);

      // --------------------------------
      // API Call
      // --------------------------------

      const response = await fetch(API_URL, {
        method: "POST",
        body: formData,
      });

      console.log(
        "Documents API Status =",
        response.status
      );

      const responseText = await response.text();

      console.log(
        "Documents API Response =",
        responseText
      );

      // --------------------------------
      // Error
      // --------------------------------

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
          `Documents Upload Failed.\n\nStatus: ${response.status}\n${errorMessage}`
        );

        return;
      }

      // --------------------------------
      // Success Response
      // --------------------------------

      let result: any = null;

      if (responseText) {
        try {
          result = JSON.parse(responseText);
        } catch {
          console.warn(
            "Documents API returned non-JSON response."
          );
        }
      }

      console.log(
        "Documents Upload Result =",
        result
      );

      alert("Documents Uploaded Successfully.");

      // Next page
      next();
    } catch (error) {
      console.error(
        "Documents Upload Connection Error =",
        error
      );

      if (error instanceof TypeError) {
        alert(
          "Server Connection Error.\n\n" +
          "PHE.API चालू आहे का ते check करा.\n" +
          "API URL: http://localhost:5014"
        );
      } else {
        alert(
          "Documents Upload करताना error आला."
        );
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="form-card">

      {/* -------------------------------- */}
      {/* Title */}
      {/* -------------------------------- */}

      <div className="form-title">
        <h2>
          कागदपत्रे / Documents Upload
        </h2>
      </div>

      <div className="title-border"></div>

      {/* -------------------------------- */}
      {/* Documents */}
      {/* -------------------------------- */}

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
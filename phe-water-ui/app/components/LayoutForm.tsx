"use client";

import { useState } from "react";

type Props = {
  next: () => void;
  back: () => void;
  applicationNo: string;
};

export default function LayoutForm({
  next,
  back,
  applicationNo,
}: Props) {
  const [applicationType, setApplicationType] = useState("");
  const [peth, setPeth] = useState("");
  const [zone, setZone] = useState("");
  const [propertyNumber, setPropertyNumber] = useState("");
  const [approvedLayoutNumber, setApprovedLayoutNumber] = useState("");
  const [approvedLayoutDate, setApprovedLayoutDate] = useState("");
  const [layoutAddress, setLayoutAddress] = useState("");

  const [loading, setLoading] = useState(false);

  // =========================================================
  // VALIDATION
  // =========================================================

  const validateLayout = () => {
    if (!applicationNo || applicationNo.trim() === "") {
      alert("Application Number मिळाला नाही.");
      return false;
    }

    if (!applicationType || applicationType.trim() === "") {
      alert("कृपया अर्जाचा प्रकार निवडा.");
      return false;
    }

    if (!peth || peth.trim() === "") {
      alert("कृपया पेठ निवडा.");
      return false;
    }

    if (!zone || zone.trim() === "") {
      alert("कृपया झोन निवडा.");
      return false;
    }

    if (!propertyNumber.trim()) {
      alert("कृपया मिळकत क्रमांक भरा.");
      return false;
    }

    if (!approvedLayoutNumber.trim()) {
      alert("कृपया मंजूर लेआउट क्रमांक भरा.");
      return false;
    }

    if (!approvedLayoutDate) {
      alert("कृपया मंजूर लेआउट दिनांक निवडा.");
      return false;
    }

    // Date validation
    const selectedDate = new Date(
      `${approvedLayoutDate}T00:00:00`
    );

    const today = new Date();

    today.setHours(0, 0, 0, 0);

    if (selectedDate > today) {
      alert("मंजूर लेआउट दिनांक आजच्या तारखेपेक्षा पुढील असू शकत नाही.");
      return false;
    }

    if (!layoutAddress.trim()) {
      alert("कृपया जागेचा पत्ता भरा.");
      return false;
    }

    return true;
  };

  // =========================================================
  // SAVE LAYOUT
  // =========================================================

  const handleNext = async () => {
    if (!validateLayout()) {
      return;
    }

    if (loading) {
      return;
    }

    setLoading(true);

    try {
      // IMPORTANT:
      // Controller name = ApplicantsController
      // Therefore route = /api/Applicants
      const API_URL =
        `http://localhost:5014/api/Applicants/Layout/${encodeURIComponent(
          applicationNo.trim()
        )}`;

      console.log("---------------------------------------");
      console.log("Calling Layout API:");
      console.log(API_URL);
      console.log("Application No:", applicationNo);
      console.log("Application Type:", applicationType);
      console.log("Peth:", peth);
      console.log("Zone:", zone);
      console.log("Property Number:", propertyNumber);
      console.log("Approved Layout Number:", approvedLayoutNumber);
      console.log("Approved Layout Date:", approvedLayoutDate);
      console.log("Layout Address:", layoutAddress);
      console.log("---------------------------------------");

      const response = await fetch(API_URL, {
        method: "PUT",

        headers: {
          "Content-Type": "application/json",
          Accept: "application/json",
        },

        body: JSON.stringify({
          applicationType: applicationType.trim(),
          peth: peth.trim(),
          zone: zone.trim(),
          propertyNumber: propertyNumber.trim(),
          layoutAddress: layoutAddress.trim(),
          approvedLayoutNumber:
            approvedLayoutNumber.trim(),

          // input type=date gives yyyy-MM-dd
          approvedLayoutDate: approvedLayoutDate,
        }),
      });

      // Read response only once
      const responseText = await response.text();

      console.log("Layout API Status:", response.status);
      console.log("Layout API Response:", responseText);

      // =====================================================
      // API ERROR
      // =====================================================

      if (!response.ok) {
        let errorMessage = responseText;

        try {
          const errorJson = JSON.parse(responseText);

          errorMessage =
            errorJson.message ||
            errorJson.error ||
            responseText;
        } catch {
          // Keep original response text
        }

        alert(
          `Layout Information Save Failed.\n\n` +
          `Status: ${response.status}\n\n` +
          `${errorMessage}`
        );

        return;
      }

      // =====================================================
      // SUCCESS RESPONSE
      // =====================================================

      let result: any = null;

      try {
        result = JSON.parse(responseText);
      } catch {
        console.warn(
          "Layout API returned non-JSON response:",
          responseText
        );
      }

      console.log("Layout Save Result:", result);

      alert(
        "Layout Information Saved Successfully."
      );

      // Go to next form
      next();
    } catch (error) {
      console.error(
        "Layout Connection Error:",
        error
      );

      if (error instanceof TypeError) {
        alert(
          "API Connection Failed.\n\n" +
          "कृपया PHE.API चालू आहे का ते तपासा.\n\n" +
          "Backend: http://localhost:5014"
        );
      } else {
        alert(
          "Layout Save करताना error आला.\n\n" +
          (error instanceof Error
            ? error.message
            : String(error))
        );
      }
    } finally {
      setLoading(false);
    }
  };

  // =========================================================
  // UI
  // =========================================================

  return (
    <div className="form-card">

      {/* ================================
          TITLE
      ================================= */}

      <div className="form-title">
        <h2>
          लेआउट माहिती / Layout Information
        </h2>
      </div>

      <div className="title-border"></div>

      {/* ================================
          APPLICATION NUMBER
      ================================= */}

      <div
        style={{
          marginBottom: "20px",
          padding: "12px 15px",
          background: "#e6f9ff",
          borderRadius: "8px",
          border: "1px solid #bdeeff",
        }}
      >
        <strong>
          Application No:
        </strong>{" "}
        {applicationNo || "Not Available"}
      </div>

      {/* ================================
          FORM GRID
      ================================= */}

      <div className="grid">

        {/* =================================
            APPLICATION TYPE
        ================================= */}

        <div>
          <label>
            ना हरकत दाखला प्रकार{" "}
            <span>*</span>
          </label>

          <select
            value={applicationType}
            onChange={(e) =>
              setApplicationType(e.target.value)
            }
            disabled={loading}
          >
            <option value="">
              Select / निवडा
            </option>

            <option value="Water">
              Water / पाणी
            </option>
          </select>
        </div>

        {/* =================================
            PETH
        ================================= */}

        <div>
          <label>
            पेठ <span>*</span>
          </label>

          <select
            value={peth}
            onChange={(e) =>
              setPeth(e.target.value)
            }
            disabled={loading}
          >
            <option value="">
              Select Peth
            </option>

            <option value="South Kasba / दक्षिण कसबा">
              South Kasba / दक्षिण कसबा
            </option>

            <option value="Mangalwar Peth / मंगळवार पेठ">
              Mangalwar Peth / मंगळवार पेठ
            </option>

            <option value="Goldfinch Peth / गोल्‍ड फिंच">
              Goldfinch Peth / गोल्‍ड फिंच
            </option>

            <option value="North Kasba / उत्तर कसबा">
              North Kasba / उत्तर कसबा
            </option>

            <option value="Budhwar Peth / बुधवार पेठ">
              Budhwar Peth / बुधवार पेठ
            </option>

            <option value="Bhavani Peth / भवानी पेठ">
              Bhavani Peth / भवानी पेठ
            </option>

            <option value="East Mangalvar Peth / पुर्व मंगळवार पेठ">
              East Mangalvar Peth / पुर्व मंगळवार पेठ
            </option>

            <option value="West Mangalvar Peth / पश्चिम मंगळवार पेठ">
              West Mangalvar Peth / पश्चिम मंगळवार पेठ
            </option>

            <option value="Guruvar Peth / गुरुवार पेठ">
              Guruvar Peth / गुरुवार पेठ
            </option>

            <option value="Somwar Peth / सोमवार पेठ">
              Somwar Peth / सोमवार पेठ
            </option>

            <option value="Jodbhavi Peth / जोडभावी पेठ">
              Jodbhavi Peth / जोडभावी पेठ
            </option>

            <option value="Ganesh Peth / गणेश पेठ">
              Ganesh Peth / गणेश पेठ
            </option>

            <option value="Sakhar Peth / साखर पेठ">
              Sakhar Peth / साखर पेठ
            </option>

            <option value="Ravivar Peth / रविवार पेठ">
              Ravivar Peth / रविवार पेठ
            </option>

            <option value="Shukravar Peth / शुक्रवार पेठ">
              Shukravar Peth / शुक्रवार पेठ
            </option>

            <option value="Shaniwar Peth / शनिवार पेठ">
              Shaniwar Peth / शनिवार पेठ
            </option>

            <option value="Pachha Peth / पाच्‍छा पेठ">
              Pachha Peth / पाच्‍छा पेठ
            </option>

            <option value="North Sadar Bazar / उत्तर सदर बजार">
              North Sadar Bazar / उत्तर सदर बजार
            </option>

            <option value="South Sadar Bazar / दक्षिण सदर बजार">
              South Sadar Bazar / दक्षिण सदर बजार
            </option>

            <option value="Modi / मोदी">
              Modi / मोदी
            </option>

            <option value="Railway Line / रेल्‍वे लाईन">
              Railway Line / रेल्‍वे लाईन
            </option>

            <option value="Siddheshwar Peth / सिध्‍देश्‍वर पेठ">
              Siddheshwar Peth / सिध्‍देश्‍वर पेठ
            </option>

            <option value="Begum Peth / बेगम पेठ">
              Begum Peth / बेगम पेठ
            </option>

            <option value="Murarji Peth / मुरारजी पेठ">
              Murarji Peth / मुरारजी पेठ
            </option>

            <option value="Laxmi Peth / लक्ष्‍मी पेठ">
              Laxmi Peth / लक्ष्‍मी पेठ
            </option>

            <option value="A Area / ए एरिया">
              A Area / ए एरिया
            </option>

            <option value="New Trihe-Gaon / न्‍यु ति-हे गाव">
              New Trihe-Gaon / न्‍यु ति-हे गाव
            </option>

            <option value="Civil Line / सिव्‍हिल लाईन">
              Civil Line / सिव्‍हिल लाईन
            </option>

            <option value="New A Area / न्‍यु ए एरिया">
              New A Area / न्‍यु ए एरिया
            </option>

            <option value="Hotgi Road / होटगी रोड">
              Hotgi Road / होटगी रोड
            </option>

            <option value="Vijapur Road / विजापुर रोड">
              Vijapur Road / विजापुर रोड
            </option>

            <option value="Salgar Vasti / सलगर वस्‍ती">
              Salgar Vasti / सलगर वस्‍ती
            </option>

            <option value="Vidi Gharkul 1 / विडी घरकुल 1">
              Vidi Gharkul 1 / विडी घरकुल 1
            </option>

            <option value="Vidi Gharkul 2 / विडी घरकुल 2">
              Vidi Gharkul 2 / विडी घरकुल 2
            </option>

            <option value="SHELGI-51 / शेळगी-51">
              SHELGI-51 / शेळगी-51
            </option>

            <option value="Gavasu / ग.व.सु.">
              Gavasu / ग.व.सु.
            </option>

            <option value="Shelgi-51 / शेळगी-51">
              Shelgi-51 / शेळगी-51
            </option>

            <option value="Dahitne-52 / दहीटणे-52">
              Dahitne-52 / दहीटणे-52
            </option>

            <option value="Degaon-53 / देगाव-53">
              Degaon-53 / देगाव-53
            </option>

            <option value="Kumthe-54 / कुमठे-54">
              Kumthe-54 / कुमठे-54
            </option>

            <option value="Kegaon-55 / केगाव-55">
              Kegaon-55 / केगाव-55
            </option>

            <option value="Soregaon-56 / सोरेगाव-56">
              Soregaon-56 / सोरेगाव-56
            </option>

            <option value="Bale-57 / बाळे-57">
              Bale-57 / बाळे-57
            </option>

            <option value="Kasbe Solapur-58 / कसबे सोलापूर-58">
              Kasbe Solapur-58 / कसबे सोलापूर-58
            </option>

            <option value="Majrewadi-59 / मजरेवाडी-59">
              Majrewadi-59 / मजरेवाडी-59
            </option>

            <option value="Majrewadi-60 / मजरेवाडी-60">
              Majrewadi-60 / मजरेवाडी-60
            </option>

            <option value="Majrewadi-61 / मजरेवाडी-61">
              Majrewadi-61 / मजरेवाडी-61
            </option>

            <option value="Majrewadi-62 / मजरेवाडी-62">
              Majrewadi-62 / मजरेवाडी-62
            </option>

            <option value="Majrewadi-63 / मजरेवाडी-63">
              Majrewadi-63 / मजरेवाडी-63
            </option>

            <option value="Majrewadi-64 / मजरेवाडी-64">
              Majrewadi-64 / मजरेवाडी-64
            </option>

            <option value="Majrewadi-65 / मजरेवाडी-65">
              Majrewadi-65 / मजरेवाडी-65
            </option>

            <option value="Majrewadi-66 / मजरेवाडी-66">
              Majrewadi-66 / मजरेवाडी-66
            </option>

            <option value="Majrewadi-67 / शेळगी-67">
              Majrewadi-67 / शेळगी-67
            </option>

            <option value="Majrewadi-68 / शेळगी-68">
              Majrewadi-68 / शेळगी-68
            </option>

            <option value="Soregaon-69 / सोरेगाव-69">
              Soregaon-69 / सोरेगाव-69
            </option>

            <option value="Kasbe Solapur-70 / कसबे सोलापूर-70">
              Kasbe Solapur-70 / कसबे सोलापूर-70
            </option>
          </select>
        </div>

        {/* =================================
            ZONE
        ================================= */}

        <div>
          <label>
            झोन <span>*</span>
          </label>

          <select
            value={zone}
            onChange={(e) =>
              setZone(e.target.value)
            }
            disabled={loading}
          >
            <option value="">
              Select Zone
            </option>

            <option value="Zone 1">
              Zone 1
            </option>

            <option value="Zone 2">
              Zone 2
            </option>

            <option value="Zone 3">
              Zone 3
            </option>

            <option value="Zone 4">
              Zone 4
            </option>

            <option value="Zone 5">
              Zone 5
            </option>

            <option value="Zone 6">
              Zone 6
            </option>

            <option value="Zone 7">
              Zone 7
            </option>

            <option value="Zone 8">
              Zone 8
            </option>
          </select>
        </div>

        {/* =================================
            PROPERTY NUMBER
        ================================= */}

        <div>
          <label>
            मिळकत क्रमांक <span>*</span>
          </label>

          <input
            type="text"
            placeholder="Property Number"
            value={propertyNumber}
            onChange={(e) =>
              setPropertyNumber(e.target.value)
            }
            disabled={loading}
          />
        </div>

        {/* =================================
            APPROVED LAYOUT NUMBER
        ================================= */}

        <div>
          <label>
            मंजूर लेआउट क्रमांक{" "}
            <span>*</span>
          </label>

          <input
            type="text"
            placeholder="Approved Layout Number"
            value={approvedLayoutNumber}
            onChange={(e) =>
              setApprovedLayoutNumber(
                e.target.value
              )
            }
            disabled={loading}
          />
        </div>

        {/* =================================
            APPROVED LAYOUT DATE
        ================================= */}

        <div>
          <label>
            मंजूर लेआउट दिनांक{" "}
            <span>*</span>
          </label>

          <input
            type="date"
            value={approvedLayoutDate}
            onChange={(e) =>
              setApprovedLayoutDate(
                e.target.value
              )
            }
            max={
              new Date()
                .toISOString()
                .split("T")[0]
            }
            disabled={loading}
          />
        </div>

        {/* =================================
            LAYOUT ADDRESS
        ================================= */}

        <div className="full">
          <label>
            जागेचा पत्ता <span>*</span>
          </label>

          <textarea
            placeholder="Layout Address"
            value={layoutAddress}
            onChange={(e) =>
              setLayoutAddress(e.target.value)
            }
            disabled={loading}
          ></textarea>
        </div>

      </div>

      {/* =================================
          BUTTONS
      ================================= */}

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
          onClick={handleNext}
          disabled={loading}
        >
          {loading
            ? "Saving..."
            : "Next →"}
        </button>

      </div>

    </div>
  );
}
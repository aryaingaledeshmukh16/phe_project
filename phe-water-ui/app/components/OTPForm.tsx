"use client";

import { useState } from "react";

type Props = {
  applicationNo: string;
  back: () => void;
};

export default function OTPForm({
  applicationNo,
  back,
}: Props) {
  const [otp, setOtp] = useState("");
  const [generatedOtp, setGeneratedOtp] = useState("");
  const [loading, setLoading] = useState(false);
  const [verified, setVerified] = useState(false);

  console.log("OTP applicationNo =", applicationNo);

  // =====================================================
  // SEND OTP
  // =====================================================

  const sendOtp = async () => {

    if (!applicationNo) {
      alert("Application Number मिळाला नाही.");
      return;
    }

    try {
      setLoading(true);

      const response = await fetch(
        `http://localhost:5014/api/Applicant/SendOtp/${applicationNo}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
        }
      );

      const result = await response.json();

      console.log("Send OTP Result:", result);

      if (!response.ok) {
        alert(result.message || "OTP generate failed.");
        return;
      }

      // DEVELOPMENT PURPOSE
      setGeneratedOtp(result.otp);

      alert(
        `OTP तुमच्या mobile number वर पाठविला आहे.\n\nDevelopment OTP: ${result.otp}`
      );

    } catch (error) {

      console.error("Send OTP Error:", error);

      alert("Server Connection Error.");

    } finally {
      setLoading(false);
    }
  };


  // =====================================================
  // VERIFY OTP
  // =====================================================

  const verifyOtp = async () => {

    if (!applicationNo) {
      alert("Application Number मिळाला नाही.");
      return;
    }

    if (otp.trim() === "") {
      alert("कृपया OTP भरा.");
      return;
    }

    if (otp.length !== 6) {
      alert("कृपया 6 digit OTP भरा.");
      return;
    }

    try {

      setLoading(true);

      const response = await fetch(
        `http://localhost:5014/api/Applicant/VerifyOtp/${applicationNo}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            otp: otp,
          }),
        }
      );

      const result = await response.json();

      console.log("Verify OTP Result:", result);

      if (!response.ok) {

        alert(result.message || "OTP verification failed.");

        return;
      }

      setVerified(true);

      alert("OTP Verified Successfully.");

    } catch (error) {

      console.error("Verify OTP Error:", error);

      alert("Server Connection Error.");

    } finally {

      setLoading(false);

    }
  };


  // =====================================================
  // SUCCESS SCREEN
  // =====================================================

  if (verified) {

    return (
      <div className="form-card">

        <div className="form-title">
          <h2>
            अर्ज यशस्वीरीत्या सादर झाला
          </h2>
        </div>

        <div className="title-border"></div>

        <div
          style={{
            textAlign: "center",
            padding: "40px",
          }}
        >

          <h2 style={{ color: "green" }}>
            ✓ Application Submitted Successfully
          </h2>

          <br />

          <h3>
            Application Number
          </h3>

          <h2 style={{ color: "#b51d5c" }}>
            {applicationNo}
          </h2>

          <br />

          <p>
            तुमचा अर्ज यशस्वीरीत्या सादर झाला आहे.
          </p>

        </div>

      </div>
    );
  }


  // =====================================================
  // OTP FORM
  // =====================================================

  return (
    <div className="form-card">

      <div className="form-title">

        <h2>
          OTP पडताळणी / OTP Verification
        </h2>

      </div>

      <div className="title-border"></div>


      <div className="grid">

        {/* Application Number */}

        <div className="full">

          <label>
            Application Number
          </label>

          <input
            type="text"
            value={applicationNo}
            readOnly
          />

        </div>


        {/* Send OTP */}

        <div className="full">

          <button
            type="button"
            className="next-btn"
            onClick={sendOtp}
            disabled={loading}
          >
            {loading
              ? "Sending..."
              : "Send OTP"}
          </button>

        </div>


        {/* Development OTP */}

        {generatedOtp && (

          <div className="full">

            <label>
              Development OTP
            </label>

            <input
              type="text"
              value={generatedOtp}
              readOnly
            />

          </div>

        )}


        {/* OTP */}

        <div className="full">

          <label>
            OTP <span>*</span>
          </label>

          <input
            type="text"
            maxLength={6}
            placeholder="Enter 6 digit OTP"
            value={otp}
            onChange={(e) =>
              setOtp(
                e.target.value.replace(/\D/g, "")
              )
            }
          />

        </div>

      </div>


      {/* Buttons */}

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
          onClick={verifyOtp}
          disabled={loading}
        >
          {loading
            ? "Verifying..."
            : "Verify OTP →"}
        </button>

      </div>

    </div>
  );
}
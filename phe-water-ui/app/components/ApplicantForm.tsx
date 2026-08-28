"use client";

import { useState } from "react";

type Props = {
  next: (applicationNo: string) => void;
};

export default function ApplicantForm({ next }: Props) {
  const [fullName, setFullName] = useState("");
  const [mobile, setMobile] = useState("");
  const [email, setEmail] = useState("");
  const [aadhaar, setAadhaar] = useState("");
  const [address, setAddress] = useState("");

  const validateStep1 = () => {
    if (fullName.trim() === "") {
      alert("कृपया संपूर्ण नाव भरा.");
      return false;
    }

    if (!/^[789]\d{9}$/.test(mobile)) {
      alert("कृपया योग्य 10 अंकी मोबाईल नंबर भरा.");
      return false;
    }

    if (email.trim() === "") {
      alert("कृपया ईमेल आयडी भरा.");
      return false;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (!emailRegex.test(email.trim())) {
      alert("कृपया योग्य ईमेल आयडी भरा.");
      return false;
    }

    if (!/^\d{12}$/.test(aadhaar)) {
      alert("कृपया योग्य 12 अंकी आधार क्रमांक भरा.");
      return false;
    }

    if (address.trim() === "") {
      alert("कृपया पत्ता भरा.");
      return false;
    }

    return true;
  };

  const handleNext = async () => {
    if (!validateStep1()) {
      return;
    }

    try {
      const API_URL = "http://localhost:5014/api/Applicants";

      console.log("Calling Applicant API:", API_URL);

     const response = await fetch(API_URL, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          fullName: fullName.trim(),
          mobileNumber: mobile.trim(),
          email: email.trim(),
          aadhaarNumber: aadhaar.trim(),
          address: address.trim(),

          applicationType: "",
          peth: "",
          zone: "",
          propertyNumber: "",
          layoutAddress: "",
          approvedLayoutNumber: "",
          approvedLayoutDate: null,
        }),
      });

      const responseText = await response.text();

      console.log("Applicant API Status:", response.status);
      console.log("Applicant API Response:", responseText);

      if (!response.ok) {
        alert(
          `API Error ${response.status}: ${
            responseText || "No response from server."
          }`
        );
        return;
      }

      let result: any = null;

      try {
        result = JSON.parse(responseText);
      } catch (error) {
        console.error("JSON Parse Error:", error);

        alert(
          "API returned an invalid response:\n\n" +
            responseText
        );

        return;
      }

      console.log("Applicant Save Result:", result);

      if (!result?.applicationNo) {
        alert(
          "Application Number API मधून मिळाला नाही.\n\n" +
            JSON.stringify(result, null, 2)
        );

        return;
      }

      alert(
        "Application Saved Successfully.\n\n" +
          "Application No: " +
          result.applicationNo
      );

      next(result.applicationNo);
    } catch (error) {
      console.error("Applicant Connection Error:", error);

      alert(
        "API Connection Failed.\n\n" +
          "Please check whether PHE.API is running on:\n" +
          "http://localhost:5014\n\n" +
          "Error: " +
          (error instanceof Error
            ? error.message
            : String(error))
      );
    }
  };

  const handleReset = () => {
    setFullName("");
    setMobile("");
    setEmail("");
    setAadhaar("");
    setAddress("");
  };

  return (
    <div className="form-card">
      <div className="form-title">
        <h2>अर्जदार माहिती / Applicant Information</h2>
      </div>

      <div className="title-border"></div>

      <div className="grid">
        {/* Full Name */}
        <div>
          <label>
            संपूर्ण नाव <span>*</span>
          </label>

          <input
            type="text"
            placeholder="Full Name"
            value={fullName}
            onChange={(e) => setFullName(e.target.value)}
          />
        </div>

        {/* Mobile Number */}
        <div>
          <label>
            मोबाईल नंबर <span>*</span>
          </label>

          <input
            type="tel"
            placeholder="Mobile Number"
            maxLength={10}
            value={mobile}
            onChange={(e) => {
              const value = e.target.value.replace(/\D/g, "");
              setMobile(value);
            }}
          />
        </div>

        {/* Email */}
        <div>
          <label>
            ईमेल आयडी <span>*</span>
          </label>

          <input
            type="email"
            placeholder="Email ID"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </div>

        {/* Aadhaar */}
        <div>
          <label>
            आधार क्रमांक <span>*</span>
          </label>

          <input
            type="text"
            placeholder="Aadhaar Number"
            maxLength={12}
            value={aadhaar}
            onChange={(e) => {
              const value = e.target.value.replace(/\D/g, "");
              setAadhaar(value);
            }}
          />
        </div>

        {/* Address */}
        <div className="full">
          <label>
            पत्ता <span>*</span>
          </label>

          <textarea
            placeholder="Address"
            value={address}
            onChange={(e) => setAddress(e.target.value)}
          />
        </div>
      </div>

      {/* Buttons */}
      <div className="btn-area">
        <button
          type="button"
          className="next-btn"
          onClick={handleNext}
        >
          Next →
        </button>

        <button
          type="button"
          className="reset-btn"
          onClick={handleReset}
        >
          Reset
        </button>
      </div>
    </div>
  );
}
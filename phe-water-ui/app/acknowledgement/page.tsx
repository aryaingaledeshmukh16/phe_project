"use client";

export const dynamic = "force-dynamic";

import { useEffect, useState } from "react";
import Acknowledgement from "../components/Acknowledgement";

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

export default function AcknowledgementPage() {
  const [applicationNo, setApplicationNo] = useState<string | null>(null);

  const [data, setData] = useState<AcknowledgementData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    // get applicationNo from URL (client-side) to avoid next/navigation SSR issues
    const params = typeof window !== "undefined" ? new URLSearchParams(window.location.search) : null;
    const appNo = params?.get("applicationNo") ?? null;

    if (!appNo) {
      setError("Application Number मिळाला नाही.");
      setLoading(false);
      return;
    }

    setApplicationNo(appNo);
  }, []);

  const formatDate = (
    date: string | null | undefined
  ) => {
    if (!date) return "-";

    return new Date(date).toLocaleDateString(
      "en-IN",
      {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
      }
    );
  };


  
  /* =========================
     LOADING
  ========================= */

  if (error) {
    return (
      <div className="ack-page">
        <div className="ack-card error-card">
          <h2>Acknowledgement Error</h2>
          <p>{error}</p>
        </div>
      </div>
    );
  }

  if (!applicationNo) return null;

  return (
    <Acknowledgement applicationNo={applicationNo} back={() => { window.location.href = "/"; }} />
  );
}



"use client";

import { useState } from "react";

import Header from "./components/Header";
import Stepper from "./components/Stepper";
import ApplicantForm from "./components/ApplicantForm";
import LayoutForm from "./components/LayoutForm";
import DocumentsForm from "./components/DocumentsForm";

import Acknowledgement from "./components/Acknowledgement";

export default function Home() {
  const [step, setStep] = useState(1);

  const [applicationNo, setApplicationNo] = useState("");

  
  console.log("PAGE applicationNo =", applicationNo);

  return (
  <div className="page-wrapper">

    <Header />

    <Stepper currentStep={step} />

    {/* STEP 1 */}
    {step === 1 && (
      <ApplicantForm
        next={(applicationNo: string) => {
          console.log("Application No from Step 1:", applicationNo);

          setApplicationNo(applicationNo);
          setStep(2);
        }}
      />
    )}

    {/* STEP 2 */}
    {step === 2 && (
      <LayoutForm
        applicationNo={applicationNo}
        back={() => setStep(1)}
        next={() => setStep(3)}
      />
    )}

    {/* STEP 3 */}
   {step === 3 && (
  <DocumentsForm
    applicationNo={applicationNo}
    next={() => {
      window.location.href =
        `/acknowledgement?applicationNo=${applicationNo}`;
    }}
    back={() => setStep(2)}
  />
)}

   

  </div>
);
}
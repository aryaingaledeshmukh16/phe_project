"use client";

import { useState } from "react";

import Stepper from "./components/Stepper";
import LayoutForm from "./components/LayoutForm";
import DocumentsForm from "./components/DocumentsForm";
import Acknowledgement from "./components/Acknowledgement";


type LayoutData = {
  applicationType: string;
  peth: string;
  zone: string;
  propertyNumber: string;
  approvedLayoutNumber: string;
  approvedLayoutDate: string;
  layoutAddress: string;
};



export default function Home() {
  const [step, setStep] = useState(1);
  const [applicationNo, setApplicationNo] = useState("");
  const [layoutData, setLayoutData] = useState<LayoutData>({
    applicationType: "",
    peth: "",
    zone: "",
    propertyNumber: "",
    approvedLayoutNumber: "",
    approvedLayoutDate: "",
    layoutAddress: "",
  });

  return (
    <div className="page-wrapper">
      <Stepper currentStep={step} />

      {step === 1 && (
        <LayoutForm
          applicationNo={applicationNo}
          layoutData={layoutData}
          onChange={setLayoutData}
          back={() => setStep(1)}
          next={(data) => {
            setLayoutData(data);
            setStep(2);
          }}
        />
      )}

      {step === 2 && (
        <DocumentsForm
          layoutData={layoutData}
          next={(newApplicationNo: string) => {
            setApplicationNo(newApplicationNo);
            setStep(3);
          }}
          back={() => setStep(1)}
        />
      )}

      {step === 3 && (
        <Acknowledgement
          applicationNo={applicationNo}
          back={() => setStep(2)}
        />
      )}
    </div>
  );
}




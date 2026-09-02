type StepperProps = {
  currentStep?: number;
};

export default function Stepper({
  currentStep = 1,
}: StepperProps) {
  const steps = [
    "लेआउट माहिती",
    "कागदपत्रे",
    "अर्जाची पोचपावती",
  ];

  return (
    <div className="stepper-card">
      <div className="stepper">
        {steps.map((step, index) => (
          <div className="step-item" key={index}>
            {index !== 0 && (
              <div
                className={
                  index < currentStep
                    ? "step-line active-line"
                    : "step-line"
                }
              />
            )}

            <div
              className={
                currentStep === index + 1
                  ? "step-box active"
                  : currentStep > index + 1
                  ? "step-box completed"
                  : "step-box"
              }
            >
              {index + 1}
            </div>

            <span
              className={
                currentStep === index + 1
                  ? "step-title active-title"
                  : currentStep > index + 1
                  ? "step-title completed-title"
                  : "step-title"
              }
            >
              {step}
            </span>
          </div>
        ))}
      </div>
    </div>
  );
}
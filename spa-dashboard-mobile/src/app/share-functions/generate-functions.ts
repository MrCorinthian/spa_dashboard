export const GenerateMonthList = () => {
  const monthNames: string[] = [];

  for (let monthIndex = 0; monthIndex < 12; monthIndex++) {
    const date = new Date(2013, monthIndex);
    const monthName = date.toLocaleString('en-EN', { month: 'long' });
    monthNames.push(monthName);
  }

  return monthNames;
};

export const GenerateYearList = () => {
  const date = new Date();
  const Years: string[] = [];
  const startYear = 2023;
  for (let i = startYear; i <= date.getFullYear(); i++) {
    Years.push(`${i}`);
  }

  return Years;
};

export const GenerateStatusList = () => {
  return ['All', 'Enable', 'Disable'];
};

export const GeneratePaymentStatusList = () => {
  return ['All', 'Completed', 'Not completed'];
};

export const GenerateCompanyTypeOfUsageList = () => {
  return ['All', 'Individual', 'Company'];
};

export const GenerateCompanyTypeOfUsageListForEdit = () => {
  return ['Individual', 'Company'];
};

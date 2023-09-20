import { MobileDropdown } from '../models/data/MobileDropdown';

export const GenerateMonthList = (): Array<MobileDropdown> => {
  const monthNames: Array<MobileDropdown> = [];

  for (let monthIndex = 0; monthIndex < 12; monthIndex++) {
    const date = new Date(2013, monthIndex);
    const monthName = date.toLocaleString('en-EN', { month: 'long' });
    monthNames.push({
      Id: 0,
      GroupName: 'MONTH',
      Value: monthName,
      Active: 'Y',
    });
  }

  return monthNames;
};

export const GenerateYearList = (): Array<MobileDropdown> => {
  const date = new Date();
  const Years: Array<MobileDropdown> = [];
  const startYear = 2023;
  for (let i = startYear; i <= date.getFullYear(); i++) {
    Years.push({
      Id: 0,
      GroupName: 'MONTH',
      Value: `${i}`,
      Active: 'Y',
    });
  }

  return Years;
};

export const GenerateStatusList = (): Array<MobileDropdown> => {
  return [
    { Id: 0, GroupName: 'STATUS', Value: 'All', Active: 'Y' },
    { Id: 0, GroupName: 'STATUS', Value: 'Enable', Active: 'Y' },
    { Id: 0, GroupName: 'STATUS', Value: 'Disable', Active: 'Y' },
  ];
};

export const GeneratePaymentStatusList = (): Array<MobileDropdown> => {
  return [
    { Id: 0, GroupName: 'PAYMENT_STATUS', Value: 'All', Active: 'Y' },
    { Id: 0, GroupName: 'PAYMENT_STATUS', Value: 'Completed', Active: 'Y' },
    { Id: 0, GroupName: 'PAYMENT_STATUS', Value: 'Not completed', Active: 'Y' },
  ];
};

export const GenerateListWithAllOption = (): Array<MobileDropdown> => {
  return [{ Id: 0, GroupName: 'ALL', Value: 'All', Active: 'Y' }];
};

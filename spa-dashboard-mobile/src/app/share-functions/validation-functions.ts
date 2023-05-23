export const EmailValidation = (value: string): boolean => {
  if (!value) return false;
  const rpEmail = value.replace(/\s/g, '');
  const emails = rpEmail.split(',');
  for (let item of emails) {
    if (item) {
      const re =
        /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
      const res = re.test(item);
      if (!res) return false;
    }
  }
  return true;
};

export const PhoneValidation = (value: string): boolean => {
  if (!value) return false;
  const rpTel = value.replace(/\s/g, '');
  const tels = rpTel.split(',');
  for (let item of tels) {
    if (item) {
      let tmpNumber = '';
      for (let item of value) {
        const cN = Number(item);
        if ((cN || cN === 0) && typeof cN === 'number') {
          tmpNumber += item;
        }
      }
      if (tmpNumber.length !== 10 && tmpNumber.length !== 9) {
        return false;
      }
    }
  }
  return true;
};

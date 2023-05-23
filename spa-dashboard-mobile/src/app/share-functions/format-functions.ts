export const Format = (type: string, value?: string) => {
  if (value) {
    if (type === 'idCard') {
      if (value.length === 13) {
        return `${value.substring(0, 1)} ${value.substring(
          1,
          5
        )} ${value.substring(5, 10)} ${value.substring(
          10,
          12
        )} ${value.substring(12, 13)}`;
      }
    } else if (type === 'phone') {
      if (value.length === 10) {
        return `${value.substring(0, 3)}-${value.substring(
          3,
          6
        )}-${value.substring(6, 10)}`;
      } else if (value.length === 9) {
        return `${value.substring(0, 2)}-${value.substring(
          2,
          5
        )}-${value.substring(5, 9)}`;
      }
    } else if (type === 'bank') {
      if (value.length === 10) {
        return `${value.substring(0, 3)}-${value.substring(
          3,
          4
        )}-${value.substring(4, 9)}-${value.substring(9, 10)} ${value.substring(
          12,
          13
        )}`;
      }
    } else if (type === 'number') {
      return Number.parseFloat(value).toLocaleString(undefined, {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      });
    }
  }

  return value;
};

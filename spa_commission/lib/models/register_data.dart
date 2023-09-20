class RegisterData {
  String? Password;
  String? FirstName;
  String? LastName;
  String? IdCardNumber;
  String? Birthday;
  String? Nationality;
  String? Address;
  String? Province;
  String? Occupation;
  String? PhoneNumber;
  String? Email;
  String? LineId;
  String? WhatsAppId;
  String? CompanyTypeOfUsage;
  String? CompanyName;
  String? CompanyTaxId;
  String? CompanyAddress;
  String? Bank;
  String? BankAccountNumber;
  String? ProfilePath;
  String? IdCardPath;

  RegisterData(
      {this.Password,
      this.FirstName,
      this.LastName,
      this.IdCardNumber,
      this.Birthday,
      this.Nationality,
      this.Address,
      this.Province,
      this.Occupation,
      this.PhoneNumber,
      this.Email,
      this.LineId,
      this.WhatsAppId,
      this.CompanyTypeOfUsage,
      this.CompanyName,
      this.CompanyTaxId,
      this.CompanyAddress,
      this.Bank,
      this.BankAccountNumber,
      this.ProfilePath,
      this.IdCardPath});

  Map<String, dynamic> toJson() => {
        'Password': Password,
        'FirstName': FirstName,
        'LastName': LastName,
        'IdCardNumber': IdCardNumber,
        'Birthday': Birthday,
        'Nationality': Nationality,
        'Address': Address,
        'Province': Province,
        'Occupation': Occupation,
        'PhoneNumber': PhoneNumber,
        'Email': Email,
        'LineId': LineId,
        'WhatsAppId': WhatsAppId,
        'CompanyTypeOfUsage': CompanyTypeOfUsage,
        'CompanyName': CompanyName,
        'CompanyTaxId': CompanyTaxId,
        'CompanyAddress': CompanyAddress,
        'Bank': Bank,
        'BankAccountNumber': BankAccountNumber,
        'ProfilePath': ProfilePath,
        'IdCardPath': IdCardPath,
      };
}

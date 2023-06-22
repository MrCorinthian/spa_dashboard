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
  String? companyTypeOfUsage;
  String? CompanyName;
  String? CompanyTexId;
  String? BankAccount;
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
      this.companyTypeOfUsage,
      this.CompanyName,
      this.CompanyTexId,
      this.BankAccount,
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
        'companyTypeOfUsage': companyTypeOfUsage,
        'CompanyName': CompanyName,
        'CompanyTexId': CompanyTexId,
        'BankAccount': BankAccount,
        'BankAccountNumber': BankAccountNumber,
        'ProfilePath': ProfilePath,
        'IdCardPath': IdCardPath,
      };
}

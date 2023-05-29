class RegisterData {
  String? Password;
  String? FirstName;
  String? LastName;
  String? IdCardNumber;
  String? Nationality;
  String? Address;
  String? Province;
  String? Occupation;
  String? PhoneNunber;
  String? Email;
  String? LineId;
  String? WhatsAppId;
  String? CompanyName;
  String? CompanyTexId;
  String? BankAccount;
  String? BankAccountNumber;
  String? ProfilePath;

  RegisterData(
      {this.Password,
      this.FirstName,
      this.LastName,
      this.IdCardNumber,
      this.Nationality,
      this.Address,
      this.Province,
      this.Occupation,
      this.PhoneNunber,
      this.Email,
      this.LineId,
      this.WhatsAppId,
      this.CompanyName,
      this.CompanyTexId,
      this.BankAccount,
      this.BankAccountNumber,
      this.ProfilePath});

  Map<String, dynamic> toJson() => {
        'Password': Password,
        'FirstName': FirstName,
        'LastName': LastName,
        'IdCardNumber': IdCardNumber,
        'Nationality': Nationality,
        'Address': Address,
        'Province': Province,
        'Occupation': Occupation,
        'PhoneNunber': PhoneNunber,
        'Email': Email,
        'LineId': LineId,
        'WhatsAppId': WhatsAppId,
        'CompanyName': CompanyName,
        'CompanyTexId': CompanyTexId,
        'BankAccount': BankAccount,
        'BankAccountNumber': BankAccountNumber,
        'ProfilePath': ProfilePath,
      };
}

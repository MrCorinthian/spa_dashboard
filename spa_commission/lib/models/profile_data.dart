class ProfileData {
  String? Token;
  String? Username;
  String? Password;
  String? TitleName;
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
  String? CompanyName;
  String? CompanyTexId;
  String? BankAccount;
  String? BankAccountNumber;
  String? ProfilePath;
  String? IdCardPath;

  ProfileData(
      {this.Password,
      this.Username,
      this.Token,
      this.TitleName,
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
      this.CompanyName,
      this.CompanyTexId,
      this.BankAccount,
      this.BankAccountNumber,
      this.ProfilePath,
      this.IdCardPath});

  Map<String, dynamic> toJson() => {
        'Password': Password,
        'Username': Username,
        'Token': Token,
        'TitleName': TitleName,
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
        'CompanyName': CompanyName,
        'CompanyTexId': CompanyTexId,
        'BankAccount': BankAccount,
        'BankAccountNumber': BankAccountNumber,
        'ProfilePath': ProfilePath,
        'IdCardPath': IdCardPath,
      };
}

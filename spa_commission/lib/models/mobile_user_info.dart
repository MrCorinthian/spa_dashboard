import 'dart:convert';

class MobileUserInfo {
  String? Token;
  String? Username;
  String? FirstName;
  String? LastName;
  String? IdCardNumber;
  String? Nationality;
  DateTime? Birthday;
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
  String? BankAccount;
  String? BankAccountNumber;
  String? ProfilePath;
  String? TierName;
  String? TierColor;
  double TotalBaht;
  double MaxBaht;

  MobileUserInfo({
    this.Token,
    this.Username,
    this.FirstName,
    this.LastName,
    this.IdCardNumber,
    this.Nationality,
    this.Birthday,
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
    this.BankAccount,
    this.BankAccountNumber,
    this.ProfilePath,
    this.TierName,
    this.TierColor,
    this.TotalBaht = 0,
    this.MaxBaht = 0,
  });

  factory MobileUserInfo.fromJson(String json) {
    Map<String, dynamic> jsonMap = jsonDecode(json);
    String? _birthday = jsonMap['Birthday'];
    DateTime? birthday;
    if (_birthday != null) birthday = DateTime.parse(_birthday);
    return MobileUserInfo(
      Token: jsonMap['Token'],
      Username: jsonMap['Username'],
      FirstName: jsonMap['FirstName'],
      LastName: jsonMap['LastName'],
      IdCardNumber: jsonMap['IdCardNumber'],
      Nationality: jsonMap['Nationality'],
      Birthday: birthday,
      Address: jsonMap['Address'],
      Province: jsonMap['Province'],
      Occupation: jsonMap['Occupation'],
      PhoneNumber: jsonMap['PhoneNumber'],
      Email: jsonMap['Email'],
      LineId: jsonMap['LineId'],
      WhatsAppId: jsonMap['WhatsAppId'],
      CompanyTypeOfUsage: jsonMap['CompanyTypeOfUsage'],
      CompanyName: jsonMap['CompanyName'],
      CompanyTaxId: jsonMap['CompanyTaxId'],
      CompanyAddress: jsonMap['CompanyAddress'],
      BankAccount: jsonMap['BankAccount'],
      BankAccountNumber: jsonMap['BankAccountNumber'],
      ProfilePath: jsonMap['ProfilePath'],
      TierName: jsonMap['TierName'],
      TierColor: jsonMap['TierColor'],
      TotalBaht: jsonMap['TotalBaht'],
      MaxBaht: jsonMap['MaxBaht'],
    );
  }
}

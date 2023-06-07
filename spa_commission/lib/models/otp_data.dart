import 'dart:convert';

class OtpData {
  String Ref;
  String Otp;
  String PhoneNumber;

  OtpData({
    this.Ref = '',
    this.Otp = '',
    this.PhoneNumber = '',
  });
  Map<String, dynamic> toJson() => {
        'Ref': Ref,
        'Otp': Otp,
        'PhoneNumber': PhoneNumber,
      };

  factory OtpData.fromJson(String json) {
    Map<String, dynamic> jsonMap = jsonDecode(json);
    return OtpData(
      Ref: jsonMap['Ref'],
      Otp: jsonMap['Otp'],
    );
  }
}

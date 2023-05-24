import 'dart:convert';

class UserReport {
  double TotalBath;
  DateTime Created;
  String PaymentStatus;

  UserReport({
    required this.TotalBath,
    required this.Created,
    required this.PaymentStatus,
  });

  factory UserReport.fromJson(Map<String, dynamic> jsonMap) {
    String? _created = jsonMap['Created'];
    DateTime created = DateTime.now();
    if (_created != null) created = DateTime.parse(_created);
    return UserReport(
      TotalBath: jsonMap['TotalBaht'],
      Created: created,
      PaymentStatus: jsonMap['PaymentStatus'],
    );
  }
}

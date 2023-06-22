import 'dart:convert';

class ResponsedData {
  bool success;
  String message;
  String data;

  ResponsedData({
    this.success = false,
    this.message = '',
    this.data = '',
  });

  factory ResponsedData.fromJson(String json) {
    Map<String, dynamic> jsonMap = jsonDecode(json);
    return ResponsedData(
      success: jsonMap['Success'],
      message: jsonMap['Message'] ?? '',
      data: jsonMap['Data'] ?? '',
    );
  }
}

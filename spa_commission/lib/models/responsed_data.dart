import 'dart:convert';

class ResponsedData {
  bool success;
  String data;

  ResponsedData({
    this.success = false,
    this.data = '',
  });

  factory ResponsedData.fromJson(String json) {
    Map<String, dynamic> jsonMap = jsonDecode(json);
    return ResponsedData(
      success: jsonMap['Success'],
      data: jsonMap['Data'] ?? '',
    );
  }
}

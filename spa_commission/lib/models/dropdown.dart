import 'dart:convert';

class Dropdown {
  int Id;
  String? GroupName;
  String? Value;
  String? Active;

  Dropdown({
    this.Id = 0,
    this.GroupName = null,
    this.Value = null,
    this.Active = null,
  });

  factory Dropdown.fromJson(Map<String, dynamic> jsonMap) {
    return Dropdown(
      Id: jsonMap['Id'] as int,
      GroupName: jsonMap['GroupName'] as String,
      Value: jsonMap['Value'] as String,
      Active: jsonMap['Active'] as String,
    );
  }
}

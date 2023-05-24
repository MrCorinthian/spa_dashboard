import 'dart:convert';

class CommissionTierInfo {
  int? Id;
  String TierName;
  String TierColor;
  double? ComBahtFrom;
  double? ComBahtTo;
  double? ComPercentage;

  CommissionTierInfo({
    this.Id,
    this.TierName = '',
    this.TierColor = '',
    this.ComBahtFrom,
    this.ComBahtTo,
    this.ComPercentage,
  });

  factory CommissionTierInfo.fromJson(Map<String, dynamic> jsonMap) {
    // Map<String, dynamic> jsonMap = jsonDecode(json);
    return CommissionTierInfo(
      Id: jsonMap['Id'],
      TierName: jsonMap['TierName'],
      TierColor: jsonMap['TierColor'],
      ComBahtFrom: jsonMap['ComBahtFrom'],
      ComBahtTo: jsonMap['ComBahtTo'],
      ComPercentage: jsonMap['ComPercentage'],
    );
  }
}

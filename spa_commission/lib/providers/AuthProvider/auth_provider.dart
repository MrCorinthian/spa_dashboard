import 'package:flutter/foundation.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../../models/responsed_data.dart';

class AuthProvider with ChangeNotifier {
  bool _isLoggedIn = false;

  bool get isLoggedIn => _isLoggedIn;

  void login(String? data) async {
    if (data != null && data != '') {
      ResponsedData token = ResponsedData.fromJson(data);
      final prefs = await SharedPreferences.getInstance();
      await prefs.setString('spa_login_token', token.data);
      _isLoggedIn = true;
      notifyListeners();
    }
  }

  void logout() {
    _isLoggedIn = false;
    notifyListeners();
  }
}

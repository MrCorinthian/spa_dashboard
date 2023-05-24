import 'package:flutter/material.dart';

class AppTheme {
  static final customTheme = ThemeData(
    brightness: Brightness.light,
    primaryColor: CustomTheme.primaryColor,
    scaffoldBackgroundColor: Colors.black,
  );

  static final darkTheme = ThemeData(
      // brightness: Brightness.dark,
      // primaryColor: Colors.purple,
      // // accentColor: Colors.orange,
      // scaffoldBackgroundColor: Colors.black,
      // // Define the rest of the theme properties here...
      );
}

class CustomTheme {
  static final imageIcon = (path) => Image.asset(
        path,
        fit: BoxFit.contain,
        height: 30,
      );
  static const paddingPage = EdgeInsets.symmetric(horizontal: 30, vertical: 30);
  static const darkGreyColor = Color(0xFF131313);
  static const primaryColor = Color(0xFFC5A048);
  static const secondaryColor = Color(0xFFC3AC7A);
  static const backgroundColor = Colors.black;
  static const fillColor = Color(0xFFEEEEEE);
  static const greenColor = Color(0xFF1B5E20);
  static final borderRadius = BorderRadius.circular(5);
  static const headerPageName =
      TextStyle(color: CustomTheme.primaryColor, fontSize: 24);

  static const headerPageNameGreen =
      TextStyle(color: Color(0xFF128001), fontSize: 24);
  static final dropdownDecoration = BoxDecoration(
      color: CustomTheme.fillColor, borderRadius: BorderRadius.circular(5));
  static const inputDecoration = InputDecoration(
    filled: true,
    fillColor: fillColor,
    border: OutlineInputBorder(
      borderSide: BorderSide.none,
    ),
  );
  static const inputDecorationError = InputDecoration(
    filled: true,
    fillColor: fillColor,
    enabledBorder: OutlineInputBorder(
      borderSide: BorderSide(
        color: Colors.red,
      ),
    ),
    focusedBorder: OutlineInputBorder(
        borderSide: BorderSide(
      color: Colors.red,
    )),
    border: OutlineInputBorder(
      borderSide: BorderSide.none,
    ),
  );

  static final buttonStyle_profileMenu = ElevatedButton.styleFrom(
    minimumSize: const Size.fromHeight(60),
    backgroundColor: Color(0xFF181818),
    alignment: Alignment.centerLeft,
  );
  static final buttonStyle_commissionTierInfo = ElevatedButton.styleFrom(
    minimumSize: const Size.fromHeight(60),
    backgroundColor: Color(0xFF181818),
    alignment: Alignment.centerLeft,
  );
  static const buttonTextStyle_profileMenu =
      TextStyle(fontSize: 16, color: fillColor);
  static final buttonStyle_primaryColor = ElevatedButton.styleFrom(
    minimumSize: const Size.fromHeight(60),
    backgroundColor: primaryColor,
  );
  static const buttonTextStyle_primaryColor =
      TextStyle(fontSize: 24, color: primaryColor);
  static final buttonStyle_secondaryColor = ElevatedButton.styleFrom(
    minimumSize: const Size.fromHeight(60),
    backgroundColor: secondaryColor,
  );
  static final buttonStyle_fillColor = ElevatedButton.styleFrom(
    minimumSize: const Size.fromHeight(60),
    backgroundColor: fillColor,
  );
  static const buttonTextStyle_fillColor =
      TextStyle(fontSize: 24, color: fillColor);
  static final buttonStyle_green = ElevatedButton.styleFrom(
    minimumSize: const Size.fromHeight(60),
    backgroundColor: Colors.green.shade900,
  );
  static final buttonStyle_red = ElevatedButton.styleFrom(
    minimumSize: const Size.fromHeight(60),
    backgroundColor: Colors.red.shade900,
  );
  static const textStyle_lightText = TextStyle(fontSize: 16, color: fillColor);
}

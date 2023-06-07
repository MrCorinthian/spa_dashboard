import 'package:flutter/material.dart';
import '../../app_theme/app_theme.dart';

class CustomLoading extends StatelessWidget {
  final bool isLoading;
  final Widget child;

  const CustomLoading({Key? key, required this.isLoading, required this.child})
      : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Stack(
      children: [
        child, // Display the main content
        if (isLoading)
          // Show the loading overlay if isLoading is true
          Container(
            color: Colors.black54, // Overlay background color
            child: Center(
              child: CircularProgressIndicator(
                color: CustomTheme.primaryColor,
              ),
            ),
          ),
      ],
    );
  }
}

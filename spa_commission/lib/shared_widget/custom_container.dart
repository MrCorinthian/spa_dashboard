import 'package:flutter/material.dart';
import '../app_theme/app_theme.dart';

class CustomContainer extends StatelessWidget {
  final Widget? child;

  const CustomContainer({Key? key, this.child}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Container(
      width: MediaQuery.of(context).size.width,
      padding: EdgeInsets.all(20),
      color: CustomTheme.darkGreyColor,
      child: child,
    );
  }
}

import 'package:flutter/material.dart';
import '../../app_theme/app_theme.dart';

class CustomAppBar extends StatelessWidget implements PreferredSizeWidget {
  const CustomAppBar({
    Key? key,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return AppBar(
      automaticallyImplyLeading: true,
      toolbarHeight: 100,
      centerTitle: true,
      backgroundColor: CustomTheme.darkGreyColor,
      title: Image.asset(
        'assets/images/urban-logo-crop.png',
        fit: BoxFit.contain,
        height: 60,
      ),
    );
  }

  @override
  Size get preferredSize => new Size.fromHeight(80);
}

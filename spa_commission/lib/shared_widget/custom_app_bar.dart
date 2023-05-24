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
      backgroundColor: CustomTheme.darkGreyColor,
      title: Row(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Image.asset(
            'assets/images/urban-logo-crop.png',
            fit: BoxFit.contain,
            height: 60,
          ),
        ],
      ),
    );
  }

  @override
  Size get preferredSize => new Size.fromHeight(80);
}

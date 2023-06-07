import 'package:flutter/material.dart';

class CustomPageRouteBuilder {
  static final leftToRight = (builder) => PageRouteBuilder(
        transitionDuration: const Duration(milliseconds: 100),
        transitionsBuilder: (context, animation, _, child) {
          return SlideTransition(
            position: Tween<Offset>(
              begin: const Offset(-1, 0),
              end: Offset.zero,
            ).animate(animation),
            child: child,
          );
        },
        pageBuilder: (_, __, ___) => builder,
      );

  static final bottomToTop = (builder) => PageRouteBuilder(
        transitionDuration: const Duration(milliseconds: 100),
        transitionsBuilder: (context, animation, _, child) {
          return SlideTransition(
            position: Tween<Offset>(
              begin: const Offset(0, 1),
              end: Offset.zero,
            ).animate(animation),
            child: child,
          );
        },
        pageBuilder: (_, __, ___) => builder,
      );
}

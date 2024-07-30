import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

class Breadcrumb extends StatelessWidget {
  final String location;
  final Map<String, String> pathParameters;

  const Breadcrumb({
    required this.location,
    required this.pathParameters,
    super.key,
  });

  static const Map<String, String> routeNames = {
    'identities': 'Identities',
    'tiers': 'Tiers',
    'clients': 'Clients',
    'address': 'Address',
    'deletion-process-details': 'Deletion Process Details',
    'deletion-process-audit-logs': 'Deletion Process Audit Logs',
  };

  @override
  Widget build(BuildContext context) {
    debugPrint("Location: $location");
    debugPrint("Path Paramters: $pathParameters");

    final segments = location.split('/').where((p) => p.isNotEmpty).toList();

    if (segments.isEmpty || _isTopLevelRoute(segments)) {
      return const SizedBox.shrink();
    }

    final pathBuffer = StringBuffer();
    final crumbWidgets = <Widget>[];

    for (var i = 0; i < segments.length; i++) {
      final segment = segments[i];
      final isLast = i == segments.length - 1;

      final displaySegment = _getSegmentValue(segment);
      final path = _buildPath(pathBuffer, displaySegment);

      crumbWidgets.add(
        GestureDetector(
          onTap: () {
            if (!isLast) {
              context.go(path);
            }
          },
          child: Text(
            _getReadableName(displaySegment),
            style: TextStyle(
              color: isLast ? Colors.black : Colors.blue,
              fontSize: 20,
            ),
          ),
        ),
      );

      if (!isLast) {
        crumbWidgets.add(
          const Text(
            ' / ',
            style: TextStyle(fontSize: 20),
          ),
        );
      }
    }

    return Row(children: crumbWidgets);
  }

  bool _isTopLevelRoute(List<String> segments) {
    return segments.length == 1 && ['identities', 'tiers', 'clients'].contains(segments[0]);
  }

  String _buildPath(StringBuffer pathBuffer, String segment) {
    pathBuffer.write('/$segment');
    return pathBuffer.toString();
  }

  String _getSegmentValue(String segment) {
    if (segment.startsWith(':')) {
      final key = segment.substring(1);
      return pathParameters[key] ?? segment;
    }
    return segment;
  }

  String _getReadableName(String segment) {
    final key = segment.startsWith(':') ? segment.substring(1) : segment;
    return routeNames[key] ?? key;
  }
}

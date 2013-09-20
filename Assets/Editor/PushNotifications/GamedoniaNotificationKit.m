#include "GamedoniaNotificationKit.h"


void ClearBadge()
{
    [UIApplication sharedApplication].applicationIconBadgeNumber = 0;
}


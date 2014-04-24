//
//  MBLUi.h
//  Mowbly
//
//  Created by Sathish on 10/09/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLFeature.h"

@interface MBLUi : MBLFeature <UIAlertViewDelegate> {

	UIAlertView *alertDlg;                          // Confirm dialog
  	NSString *alertCallbackId;                      // Holds the callbackId string for confirm and prompt dialogs
    
	BOOL alertDlgCancelable;                        // Tells if the confirm/prompt dialog is cancelable on app pause
	bool bDialogActive;                             // Tells if a confirm/prompt dialog is active. Stops new ones from opening.
}

@end

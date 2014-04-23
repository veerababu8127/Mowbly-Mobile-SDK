//
//  MBLPageViewController.h
//  Mowbly
//
//  Created by Sathish on 30/03/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import <UIKit/UIKit.h>

@class MBLAppDelegate;
@class MBLPage;

typedef enum {
    FORWARD = 0,
    BACKWARD
} NavigationDirection;

typedef enum {
    ANIM_IN = 0,
    ANIM_OUT
} AnimationDirection;

@interface MBLPageViewController : UIViewController <UIPopoverControllerDelegate> {

    MBLAppDelegate *appDelegate;                // App delegate object
    NSString *_cid;                             // Controller ID to identify controller in native-JS bridge
    MBLPage *prevPage;                          // Pointer to the previous page displayed before active page

    MBLPage *_activePage;                       // Pointer to page that is in display
    NSString *_homeName,*_homeUrl,*_homeData;   // Home name, url and data
    BOOL _retainControllerOnLowMemory;          // Tells if the page should be retained on low memory warning
    CGFloat _screenWidth, _screenHeight;        // Screen dimensions

    UIPopoverController *popoverUI;             // Popover controller for TextView on iPad
}

@property(nonatomic, copy)NSString* cid;
@property(nonatomic, retain) NSMutableDictionary *pages;
@property(nonatomic)BOOL retainControllerOnLowMemory;
@property(nonatomic, retain)MBLPage* activePage;
@property(nonatomic, copy)NSString* homeName;
@property(nonatomic, copy)NSString* homeUrl;
@property(nonatomic, copy)NSString* homeData;
@property(nonatomic, retain)NSMutableArray *pageStack;

- (id)initWithHome:(NSString *)homeName url:(NSString *)homeUrl;

- (id)initWithHomeAndData:(NSString *)homeName url:(NSString *)homeUrl homeData:(NSString *)pageData;

- (void)didNetworkStatusUpdate:(BOOL)connected;

- (NSString *) getPageUrl:(NSString *)url name:(NSString *)pageName;

- (MBLPage *)openPageWithName:(NSString *)pageName 
                               url:(NSString *)url 
                              data:(NSString *)pageData
                     configuration:(NSDictionary *)configuration
          retainPageInViewStack:(BOOL)bRetainPageInPageStack 
                         direction:(AnimationDirection)direction;

- (MBLPage *)getPageByName:(NSString *)pageName ;

- (void)navigateBack:(BOOL)destroy;

- (void)transitionFromPage:(MBLPage *)fromPage toPage:(MBLPage *)toPage direction:(AnimationDirection)direction duration:(float)duration;

- (void) reloadActivePage;

- (void) createPageWithName:(NSString *)pageName configuration:(NSDictionary *)theConfiguration;

- (void) openHome;

@end
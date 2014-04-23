//
//  MBLPhotoViewController.m
//  Mowbly
//
//  Created by Veerababu on 09/12/13.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.



#import "MBLPhotoViewController.h"
#import "MBLImageScrollView.h"
#import "MBLAppDelegate.h"

@interface MBLPhotoViewController ()
{
    NSUInteger _pageIndex;
    NSMutableDictionary *_dict;
}
@end

@implementation MBLPhotoViewController
+ (MBLPhotoViewController *)photoViewControllerForPageIndex:(NSUInteger)pageIndex  withData:(NSMutableDictionary *)dict
{
    
    if (pageIndex < [[dict objectForKey:@"images"] count])
    {
        return [[self alloc] initWithPageIndex:pageIndex withData:dict];
        
    }
    return nil;
}
- (id)initWithPageIndex:(NSInteger)pageIndex withData:(NSMutableDictionary *)dict
{
    self = [super initWithNibName:nil bundle:nil];
    if (self)
    {
        _pageIndex = pageIndex;
        _dict=[[NSMutableDictionary alloc]initWithDictionary:dict];
    }
    return self;
}
- (NSInteger)pageIndex
{
    return _pageIndex;
}

- (void)loadView
{
    
    MBLImageScrollView *scrollView =[[MBLImageScrollView alloc]initWithLabel];
    [scrollView setIndex: _pageIndex dataDictionary:_dict];
    scrollView.autoresizingMask = UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight;
    if (floor(NSFoundationVersionNumber) > NSFoundationVersionNumber_iOS_6_1) {
        // Load resources for iOS 6.1 or earlier
            self.edgesForExtendedLayout = UIRectEdgeNone;
    }
    self.view = scrollView;
    self.view.bounds=scrollView.bounds;
    self.view.backgroundColor=[UIColor blackColor];
    scrollView = nil;
}


// (this can also be defined in Info.plist via UISupportedInterfaceOrientations)
- (NSUInteger)supportedInterfaceOrientations
{
    return UIInterfaceOrientationMaskAllButUpsideDown;
}
@end

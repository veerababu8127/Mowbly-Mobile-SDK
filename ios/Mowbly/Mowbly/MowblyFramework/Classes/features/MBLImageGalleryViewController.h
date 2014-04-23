//
//  MBLImageGallery.m
//  Mowbly
//
//  Created by Veerababu on 09/12/13.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//
#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

@interface MBLImageGalleryViewController :UIViewController <UIPageViewControllerDataSource>

-(id)initDict:(NSMutableDictionary *)dict;

@property(nonatomic,retain)UIPageViewController *pvc;

@end

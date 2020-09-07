import { BrowserModule, HammerGestureConfig, HAMMER_GESTURE_CONFIG } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {HttpClientModule} from '@angular/common/http';
import { AppComponent } from './app.component';
import { NavComponent } from './Nav/Nav.component';
import { FormsModule } from '@angular/forms';
import { AuthService } from './_services/auth.service';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { ErrorInterceptorProvider } from './_services/error.interceptor';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TabsModule} from 'ngx-bootstrap/tabs';
import { BsDropdownModule} from 'ngx-bootstrap/dropdown';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberComponent } from './members/member/member.component';
import { PhotoEditorComponent } from './members/photo-editor/photo-editor.component';
import { MemberProfileComponent } from './members/member-profile/member-profile.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { appRoutes } from './routes';
import { RouterModule } from '@angular/router';
import { AlertifyService } from './_services/alertify.service';
import { JwtModule } from '@auth0/angular-jwt';
import { UserService } from './_services/user.service';
import { AuthGuard } from './_guards/auth.guard';
import { NgxGalleryModule } from '@kolkov/ngx-gallery';
import { MemberProfileResolver} from './_resolver/member-profile-resolver';
import { MemberListResolver} from './_resolver/member-list-resolver';
import { MemberEditResolver} from './_resolver/member-edit-resolver';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes-guards';
import { FileUploadModule } from 'ng2-file-upload';

export function tokenGetter(){
   return localStorage.getItem('token');
}

// export class CustomHammerConfig extends HammerGestureConfig  {
//    overrides = {
//        pinch: { enable: false },
//        rotate: { enable: false }
//    };
//  }

@NgModule({
   declarations: [
      AppComponent,
      NavComponent,
      HomeComponent,
      RegisterComponent,
      MemberListComponent,
      MemberEditComponent,
      ListsComponent,
      MemberComponent,
      PhotoEditorComponent,
      MemberProfileComponent,

      MessagesComponent
   ],
   imports: [
      BrowserModule,
      HttpClientModule,
      FormsModule,
      BrowserAnimationsModule,
      NgxGalleryModule,
      FileUploadModule,
      JwtModule.forRoot(
         {
            config: {
               tokenGetter,
               allowedDomains: ['localhost:5000'],
               disallowedRoutes: ['localhost:5000/api/auth']
            }
         }
         ),
         BsDropdownModule.forRoot(),
         TabsModule.forRoot(),
      RouterModule.forRoot(appRoutes)
   ],
   providers: [
      AuthService,
      AlertifyService,
      ErrorInterceptorProvider,
      AuthGuard,
      PreventUnsavedChanges,
      UserService,
      MemberProfileResolver,
      MemberListResolver,
      MemberEditResolver
      // { provide: HAMMER_GESTURE_CONFIG, useClass: CustomHammerConfig }
   ],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }

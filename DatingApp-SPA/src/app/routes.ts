import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberProfileComponent } from './members/member-profile/member-profile.component';
import { MemberProfileResolver } from './_resolver/member-profile-resolver';
import { MemberListResolver } from './_resolver/member-list-resolver';

export const appRoutes: Routes = [
    { path: 'home', component: HomeComponent },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: 'members', component: MemberListComponent,
                resolve: {users: MemberListResolver} },
            { path: 'members/:id', component: MemberProfileComponent, 
                resolve: {user: MemberProfileResolver} },
            { path: 'messages', component: MessagesComponent },
            { path: 'lists', component: ListsComponent },

        ]
    },
    { path: '**', redirectTo: 'home', pathMatch: 'full' }
]
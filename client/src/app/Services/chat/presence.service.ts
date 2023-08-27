import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, take } from 'rxjs';
import { User } from 'src/app/Models/User';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
    chatURL = environment.chatURL;
    private hubConnection?: HubConnection;
    private onlineUsersSource = new BehaviorSubject<string[]>([]);
    onlineUsers = this.onlineUsersSource.asObservable();

    constructor(private toastr: ToastrService, private router: Router){ }

    createHubConnection(user: User){
        this.hubConnection = new HubConnectionBuilder().withUrl(this.chatURL + 'presence', {
            accessTokenFactory: () => user.token
        }).withAutomaticReconnect().build();

        this.hubConnection.start().catch(error => console.error(error));
        this.hubConnection.on("UserIsOnline",  username => {
            this.onlineUsers.pipe(take(1)).subscribe({
                next: usernames => this.onlineUsersSource.next([...usernames, username])
            })
        })

        this.hubConnection.on("UserIsOffline", username => {
            this.onlineUsers.pipe(take(1)).subscribe({
                next: usernames => this.onlineUsersSource.next(usernames.filter(user => user !== username))
            })        
        });

        this.hubConnection.on("GetOnlineUsers", username => {
            this.onlineUsersSource.next(username);
        });

        this.hubConnection.on("NewMessageReceived", ({username, knownAs}) => {
            this.toastr.info(knownAs + ' has send you a new message ! Click me to view the message.').onTap.pipe(take(1)).subscribe({
                next: () => this.router.navigateByUrl('/members/' + username + '?tab=Messages')
            })
        })
    }

    stopHubConnection(){
        this.hubConnection?.stop().catch(error => console.error(error));
    }
}

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../Models/User';
import { environment } from 'src/environments/environment';
import { PresenceService } from './chat/presence.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
    //HTTP REQUESTS FOR USER ACCOUNTS
    
    apiURL: string = environment.apiURL;
    private currentUserSource = new BehaviorSubject<User | null>(null);
    currentUser = this.currentUserSource.asObservable();

    constructor(private http: HttpClient, private presenceService: PresenceService) { }

    login(model: User){
        return this.http.post<User>(this.apiURL + 'account/login', model).pipe(
            map((response: User) => {
                const user: User = response;
                if(user){
                    this.setCurrentUser(user);
                }
            })
        );
    }

    register(model: any){
        return this.http.post<User>(this.apiURL + 'account/register', model).pipe(
            map(user => {
                if(user){
                    this.setCurrentUser(user);
                }
            })
        );
    }

    setCurrentUser(user: User){
        user.roles = [];
        const roles = this.getDecodedToken(user.token).role;
        Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
        localStorage.setItem('user', JSON.stringify(user));
        this.currentUserSource.next(user);

        this.presenceService.createHubConnection(user);
    }

    loggout(){
        localStorage.removeItem('user');
        this.currentUserSource.next(null);
        this.presenceService.stopHubConnection();
    }

    getDecodedToken(token: string){
        return JSON.parse(atob(token.split('.')[1]));
    }


}

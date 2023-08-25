import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../Models/Member';
import { map, of, pipe, take } from 'rxjs';
import { UserParams } from '../Models/UserParams';
import { AccountService } from './account.service';
import { User } from '../Models/User';
import { getPaginatedResult, getPaginationHeaders } from './helpers/PaginationHelper';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
    baseURL: string = environment.apiURL;

    constructor(private http: HttpClient, private accountService: AccountService) { 

    }

    getUsersWithRoles(){
        return this.http.get<User[]>(this.baseURL + 'admin/users-with-roles');
    }

    updateUserRoles(username: string, roles: string) {
        return this.http.post<string[]>(this.baseURL + 'admin/edit-roles/' + username + '?roles=' + roles, {});
    }
}

import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../Models/Member';
import { map, of, pipe } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MembersService {

    baseURL: string = environment.apiURL;
    members: Member[] = [];

    constructor(private http: HttpClient) { }

    getMembers(){

        if(this.members.length > 0) return of(this.members);
        return this.http.get<Member[]>(this.baseURL + 'user').pipe(
            map(members => {
                this.members = members;
                return members;
            })
        );
    }

    getMember(username: string){
        const foundMember = this.members.find(member => member.username == username);
        if(foundMember) return of(foundMember);

        return this.http.get<Member>(this.baseURL + 'user/' + username);
    }

    updateMember(member: Member){
        return this.http.put(this.baseURL + 'user', member).pipe(
            map( () => {
                const index = this.members.indexOf(member);
                this.members[index] = { ...this.members[index], ...member };
            })
        );
    }
}

import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../Models/Member';
import { map, of, pipe } from 'rxjs';
import { PaginatedResult } from '../Models/Pagination';
import { UserParams } from '../Models/UserParams';

@Injectable({
  providedIn: 'root'
})
export class MembersService {

    baseURL: string = environment.apiURL;
    members: Member[] = [];

    constructor(private http: HttpClient) { }

    getMembers(userParams: UserParams){
        let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize);
        params = params.append('minAge', userParams.minAge);
        params = params.append('maxAge', userParams.maxAge);
        params = params.append('gender', userParams.gender);
        
        return this.getPaginatedResult<Member[]>(this.baseURL + 'user', params);
    }

    private getPaginatedResult<T>(url: string, params: HttpParams) {
        const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>;
        return this.http.get<T>(url, { observe: 'response', params }).pipe(
            map(response => {
                if (response.body) {
                    paginatedResult.result = response.body;
                }

                const pagination = response.headers.get('Pagination');
                if (pagination) {
                    paginatedResult.pagination = JSON.parse(pagination);
                }
                return paginatedResult;
            })
        );
    }

    getPaginationHeaders(pageNumber: number, pageSize: number){
        let params = new HttpParams();

        params = params.append('pageNumber', pageNumber);
        params = params.append('pageSize', pageSize);
        
        return params
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

    setMainPhoto(photoId: number){
        return this.http.put(this.baseURL + 'user/set-main-photo/' + photoId, {});
    }

    deletePhoto(photoId: number){
        return this.http.delete(this.baseURL + 'user/delete-photo/' + photoId);
    }
}

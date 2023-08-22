import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../Models/Member';
import { map, of, pipe, take } from 'rxjs';
import { PaginatedResult } from '../Models/Pagination';
import { UserParams } from '../Models/UserParams';
import { AccountService } from './account.service';
import { User } from '../Models/User';

@Injectable({
  providedIn: 'root'
})
export class MembersService {

    baseURL: string = environment.apiURL;
    members: Member[] = [];
    memberCache = new Map();
    user: User | undefined;
    userParams: UserParams | undefined;

    constructor(private http: HttpClient, private accountService: AccountService) { 
        this.accountService.currentUser.pipe(take(1)).subscribe({
            next: user => {
                if(user){
                    this.userParams = new UserParams(user);
                    this.user = user;
                }
            }
        })
    }

    getUserParams(){
        return this.userParams;
    }

    setUserParams(params: UserParams){
        this.userParams = params;
    }

    resetUserParams(){
        if(this.user){
            this.userParams = new UserParams(this.user);
            return this.userParams;
        }

        return;
    }

    getMembers(userParams: UserParams){
        const response = this.memberCache.get(Object.values(userParams).join('-'));
        if(response) 
            return of(response)
        

        let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize);
        params = params.append('minAge', userParams.minAge);
        params = params.append('maxAge', userParams.maxAge);
        params = params.append('gender', userParams.gender);
        params = params.append('orderBy', userParams.orderBy);

        return this.getPaginatedResult<Member[]>(this.baseURL + 'user', params).pipe(
            map(response => {
                this.memberCache.set(Object.values(userParams).join('-'), response)
                return response;            
            })
        );
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
        const member = [...this.memberCache.values()]
        .reduce((previousValues, currentValues) => previousValues.concat(currentValues.result), [])
        .find((member: Member) => member.username === username );
        if(member) return of(member);

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

    addLike(username: string){
        return this.http.post(this.baseURL + 'likes/' + username, {});
    }

    getLikes(predicate: string, pageNumber: number, pageSize: number){
        let params = this.getPaginationHeaders(pageNumber, pageSize);
        params = params.append('predicate', predicate);
        return this.getPaginatedResult<Member[]>(this.baseURL + 'likes', params);
    }
}

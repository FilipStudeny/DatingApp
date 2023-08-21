import { Component, OnInit } from '@angular/core';
import { response } from 'express';
import { Observable, take } from 'rxjs';
import { Member } from 'src/app/Models/Member';
import { IPagination } from 'src/app/Models/Pagination';
import { User } from 'src/app/Models/User';
import { UserParams } from 'src/app/Models/UserParams';
import { AccountService } from 'src/app/Services/account.service';
import { MembersService } from 'src/app/Services/members.service';

@Component({
  selector: 'app-members-list',
  templateUrl: './members-list.component.html',
  styleUrls: ['./members-list.component.css']
})
export class MembersListComponent implements OnInit {

    members$: Member[] = [];
    pagination: IPagination | undefined;
    userParams: UserParams | undefined;
    genderList = [
        {value: 'male', display: 'Male'}, 
        {value: 'female', display: 'Female'}
    ]

    constructor(private memberService: MembersService) {
        this.userParams = this.memberService.getUserParams();
    }
  
    ngOnInit(): void {
      this.loadMembers();
    }

    loadMembers(){
        if(this.userParams){
            this.memberService.setUserParams(this.userParams);
            this.memberService.getMembers(this.userParams).subscribe({
                next: response => {
                    if(response.result && response.pagination){
                        this.members$ = response.result;
                        this.pagination = response.pagination;
                    }
                }
            })
        }
    }
  
    pageChanged(event: any){
        if(this.userParams && this.userParams?.pageNumber !== event.page){
            this.userParams.pageNumber = event.page;
            this.memberService.setUserParams(this.userParams);
            this.loadMembers();
        }
    }

    resetFilters(){
        this.userParams = this.memberService.resetUserParams();
        this.loadMembers();
    }
}

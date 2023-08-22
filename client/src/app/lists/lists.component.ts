import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/Models/Member';
import { MembersService } from 'src/app/Services/members.service';
import { IPagination } from '../Models/Pagination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
    members: Member[] | undefined;
    predicate = 'liked';
    pageNumber = 1;
    pageSize = 5;
    pagination: IPagination | undefined;

    constructor(private memberService: MembersService, private toastr: ToastrService) {}

    ngOnInit(): void {
        this.loadLikes()
    }

    loadLikes(){
        this.memberService.getLikes(this.predicate, this.pageNumber, this.pageSize).subscribe({
            next: response => {
                this.members = response.result;
                this.pagination = response.pagination;
                console.log(response.pagination);
            }
        })
    }

    pageChanged(event: any){
        if(this.pageNumber !== event.page){
            this.pageNumber = event.page;
            this.loadLikes();
        }
    }
}

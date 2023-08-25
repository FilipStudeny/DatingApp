import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { User } from 'src/app/Models/User';
import { AdminService } from 'src/app/Services/admin.service';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {

    users: User[] = [];
    rolesModalRef: BsModalRef<RolesModalComponent> = new BsModalRef<RolesModalComponent>();
    availableRoles = [
        'Admin',
        'Moderator',
        'Member'
    ]

    constructor(private adminService: AdminService, private modalService: BsModalService) { }

    ngOnInit(): void {
        this.getUsersWithRoles();
    }

    
    getUsersWithRoles(){
        this.adminService.getUsersWithRoles().subscribe({
            next: users => this.users = users
        })
    }

    openRolesModal(user: User){
        const config = {
            class: 'modal-dialog-centered',
            initialState: {
                username: user.username,
                availableRoles: this.availableRoles,
                selectedRoles: [...user.roles]
            }
        }
        this.rolesModalRef = this.modalService.show(RolesModalComponent, config);
        this.rolesModalRef.onHide?.subscribe({
            next: () => {
                const selectedRoles: [] | any = this.rolesModalRef.content?.selectedRoles;
                if(!this.arrayEqual(selectedRoles, user.roles)){
                    this.adminService.updateUserRoles(user.username, selectedRoles).subscribe({
                        next: roles => user.roles = roles
                    });
                }
            }
        })
    }

    private arrayEqual(arr1: any[], arr2: any[]){
        return JSON.stringify(arr1.sort()) === JSON.stringify(arr2.sort());
    }
}

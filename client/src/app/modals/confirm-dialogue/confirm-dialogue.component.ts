import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-confirm-dialogue',
  templateUrl: './confirm-dialogue.component.html',
  styleUrls: ['./confirm-dialogue.component.css']
})
export class ConfirmDialogueComponent implements OnInit{
    title = '';
    message = '';
    btnOkText = '';
    btnCancelText = '';
    result = false;

    constructor(public modalRef: BsModalRef) {}

    ngOnInit(): void {
    }

    confirm(){
        this.result = true;
        this.modalRef.hide();
    }

    decline(){
        this.modalRef.hide();
    }

}

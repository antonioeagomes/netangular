import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {} 

  constructor(public accountService: AccountService, private router: Router, private toastr: ToastrService) { }

  ngOnInit(): void {
    
  }

  login() {
    this.accountService.login(this.model).subscribe({
      next: (response) => {
        this.toastr.success('Logged succefully')
        this.router.navigateByUrl('/members');
      },
      error: (e) => {
        // this.toastr.error(e.error)
      },
      complete: () => console.log('login request finished')
    })
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

}
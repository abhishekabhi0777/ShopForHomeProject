import { Component, OnInit,ChangeDetectorRef } from '@angular/core';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-user',
  standalone: false,
  templateUrl: './users.html',
  styleUrls: ['./users.css']
})
export class UserComponent implements OnInit {

  users: any[] = [];

  editingUserId: number | null = null;

  editUser: any = {
    userId: 0,
    fullName: '',
    email: '',
    phone: '',
    address: '',
    role: 'User',
    isActive: true
  };

  constructor(private userService: UserService,
    private cdr:ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.userService.getUsers().subscribe({
      next: (data) => {
        console.log('Users API data:', data);
        this.users = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error fetching users:', err);
        this.cdr.detectChanges();
      }
    });
  }

  startEdit(user: any): void {
    this.editingUserId = user.userId;
    this.editUser = {
      userId: user.userId,
      fullName: user.fullName || '',
      email: user.email || '',
      phone: user.phone || '',
      address: user.address || '',
      role: user.role || 'User',
      isActive: user.isActive
    };
  }

  cancelEdit(): void {
    this.editingUserId = null;
  }

  updateUser(): void {
    this.userService.updateUser(this.editUser.userId, this.editUser).subscribe({
      next: () => {
        alert('User updated successfully');
        this.editingUserId = null;
        this.loadUsers();
      },
      error: (err) => {
        console.error('Update error:', err);
        alert('Failed to update user');
      }
    });
  }

  deleteUser(userId: number): void {
    if (!confirm('Are you sure you want to delete this user?')) {
      return;
    }

    this.userService.deleteUser(userId).subscribe({
      next: () => {
        alert('User deleted successfully');
        this.loadUsers();
      },
      error: (err) => {
        console.error('Delete error:', err);
        alert('Failed to delete user');
      }
    });
  }
}
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Foundation;
using ObjCRuntime;
using UIKit;

using SocketIO;

namespace SocketIOClientSample
{
	partial class CRViewController : UIViewController, IUITableViewDataSource/*, IUITableViewDelegate, INSURLConnectionDelegate, INSURLConnectionDataDelegate*/, IUITextFieldDelegate
	{
		Client _io;
		List<MessageItem> _receivedMessage;
		List<string> _typingUsers;
		string _name;
		int _userCount;
		//NSTimer* _inputTimer;

		public CRViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			_receivedMessage = new List<MessageItem> ();
			_typingUsers = new List<string> ();
			_io = new Client ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Do any additional setup after loading the view, typically from a nib.

			loginPage.Hidden = false;
			nickName.Enabled = true;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			_io.SetSocketOpenListener (nsp => {
				Console.WriteLine ("C# managed set_socket_open_listener");
				InvokeOnMainThread (() => {
					onConnected ();
				});
			});
			_io.SetConnectionCloseListener (reason => {
				Console.WriteLine ("set_close_listener");
				InvokeOnMainThread (() => {
					onDisconnected ();
				});
			});
			_io.SetConnectionFailListener (() => {
				Console.WriteLine ("set_fail_listener");
				InvokeOnMainThread (() => {
					onDisconnected ();
				});
			});
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);

			_io.GetSocket ().OffAll ();
			_io.SetConnectionOpenListener (null);
			_io.SetConnectionCloseListener (null);
			_io.Close ();
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();

			// Dispose of any resources that can be recreated.
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);

			_io.Dispose ();
		}

		//		-(void)onNewMessage:(NSString*) message from:(NSString*) name
		//		{
		//			MessageItem *item = [[MessageItem alloc] init];
		//
		//			item.flag = [name isEqualToString:_name]?Message_You:Message_Other;
		//			item.message = item.flag == Message_You? [NSString stringWithFormat:@"%@:%@",message,name]:[NSString stringWithFormat:@"%@:%@",name,message];
		//			[_receivedMessage addObject:item];
		//			[_tableView reloadData];
		//
		//			if(![_messageField isFirstResponder])
		//			{
		//				[_tableView scrollToRowAtIndexPath:[NSIndexPath indexPathForRow:[_receivedMessage count]-1 inSection:0] atScrollPosition:UITableViewScrollPositionBottom animated:YES];
		//			}
		//		}

		//		-(void)onUserJoined:(NSString*)user participants:(NSInteger) num
		//		{
		//			_userCount = num;
		//			[self updateUser:user count:num joinOrLeft:YES];
		//		}

		//		-(void)onUserLeft:(NSString*) user participants:(NSInteger) num
		//		{
		//			[_typingUsers removeObject:user];//protective removal.
		//			[self updateTyping];
		//			_userCount = num;
		//			[self updateUser:user count:num joinOrLeft:NO];
		//		}

		//		-(void)onUserTyping:(NSString*) user
		//		{
		//			[_typingUsers addObject:user];
		//			[self updateTyping];
		//		}

		//		-(void)onUserStopTyping:(NSString*) user
		//		{
		//			[_typingUsers removeObject:user];
		//			[self updateTyping];
		//		}

		void onLogin (int numParticipants)
		{
			Console.WriteLine ("onLogin");

			_name = nickName.Text;
			_userCount = numParticipants;
			loginPage.Hidden = true;

			updateUser (null, _userCount, true);

			NSNotificationCenter.DefaultCenter.AddObserver (
				(NSString)"UIKeyboardWillShowNotification",
				notification => {
					Console.WriteLine ("UIKeyboardWillShowNotification");
					//CGFloat height = [[notification.userInfo objectForKey:UIKeyboardFrameBeginUserInfoKey] CGRectValue].size.height;
					//// Animate the current view out of the way
					//[UIView animateWithDuration:0.35 animations:^{
					//    self.messageArea.transform = CGAffineTransformMakeTranslation(0, -height);
					//}];
				},
				null);

			NSNotificationCenter.DefaultCenter.AddObserver (
				(NSString)"UIKeyboardWillHideNotification",
				notification => {
					Console.WriteLine ("UIKeyboardWillHideNotification");
					//[UIView animateWithDuration:0.35 animations:^{
					//    self.messageArea.transform = CGAffineTransformIdentity;
					//}];
				},
				null);
		}

		partial void onSend (UIButton sender)
		{
			Console.WriteLine ("onSend");

			if (messageField.Text.Length > 0 && _name.Length > 0) {
				_io.GetSocket ().Emit ("new message", messageField.Text);
				MessageItem item = new MessageItem {
					flag = MessageFlag.Message_You,
					message = string.Format ("{0}:You", messageField.Text)
				};
				_receivedMessage.Add (item);
				tableView.ReloadData ();
				tableView.ScrollToRow (NSIndexPath.FromItemSection (_receivedMessage.Count - 1, 0), UITableViewScrollPosition.Bottom, true);
			}

			messageField.Text = string.Empty;
			messageField.ResignFirstResponder ();
		}

		void onConnected ()
		{
			Console.WriteLine ("onConnected");

			_io.GetSocket ().Emit ("add user", nickName.Text);
		}

		void onDisconnected ()
		{
			Console.WriteLine ("onDisconnected");

			loginPage.Hidden = false;
			nickName.Enabled = true;

			NSNotificationCenter.DefaultCenter.RemoveObserver (
				this,
				"UIKeyboardWillShowNotification",
				null);

			NSNotificationCenter.DefaultCenter.RemoveObserver (
				this,
				"UIKeyboardWillHideNotification",
				null);
		}

		void updateUser (string user, int num, bool isJoin)
		{
			_userCount = num;
//			MessageItem *item = [[MessageItem alloc] init];
//
//			item.flag = Message_System;
//			if (user) {
//				item.message = [NSString stringWithFormat:@"%@ %@\n%@",user,isJoin?@"joined":@"left",num==1?@"there's 1 participant":[NSString stringWithFormat:@"there are %ld participants",num]];
//			}
//			else
//			{
//				item.message = [NSString stringWithFormat:@"Welcome to Socket.IO Chat-\n%@",num==1?@"there's 1 participant":[NSString stringWithFormat:@"there are %ld participants",num]];
//			}
//
//			[_receivedMessage addObject:item];
//			[_tableView reloadData];
//			if(![_messageField isFirstResponder])
//			{
//				[_tableView scrollToRowAtIndexPath:[NSIndexPath indexPathForRow:[_receivedMessage count]-1 inSection:0] atScrollPosition:UITableViewScrollPositionBottom animated:YES];
//			}
		}

		//		-(void) inputTimeout
		//		{
		//			_inputTimer = nil;
		//			_io->socket()->emit("stop typing", "");
		//		}

		//		-(BOOL)textField:(UITextField *)textField shouldChangeCharactersInRange:(NSRange)range replacementString:(NSString *)string
		//		{
		//			if(textField == self.messageField)
		//			{
		//				if(_inputTimer.valid)
		//				{
		//					[_inputTimer setFireDate:[NSDate dateWithTimeIntervalSinceNow:1.0]];
		//				}
		//				else
		//				{
		//					_io->socket()->emit("typing", "");
		//					_inputTimer = [NSTimer scheduledTimerWithTimeInterval:1.0 target:self selector:@selector(inputTimeout) userInfo:nil repeats:NO];
		//				}
		//			}
		//			return YES;
		//		}

		//		-(void)scrollViewDidScroll:(UIScrollView *)scrollView
		//		{
		//			if ([self.messageField isFirstResponder]) {
		//				[self.messageField resignFirstResponder];
		//			}
		//		}

		// Row display. Implementers should *always* try to reuse cells by setting each cell's reuseIdentifier and querying for available reusable cells with dequeueReusableCellWithIdentifier:
		// Cell gets various attributes set automatically based on table (separators) and data source (accessory views, editing controls)
		[Export ("tableView:cellForRowAtIndexPath:")]
		public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			MessageItem item = _receivedMessage [indexPath.Row];

			UITableViewCell cell = tableView.DequeueReusableCell ("Msg");
			cell.TextLabel.Text = item.message;
			switch (item.flag) {
			case MessageFlag.Message_System:
				cell.TextLabel.TextAlignment = UITextAlignment.Center;
				cell.TextLabel.Font = UIFont.FromName (cell.TextLabel.Font.Name, 12);
				break;
			case MessageFlag.Message_Other:
				cell.TextLabel.TextAlignment = UITextAlignment.Left;
				cell.TextLabel.Font = UIFont.FromName (cell.TextLabel.Font.Name, 15);
				break;
			case MessageFlag.Message_You:
				cell.TextLabel.TextAlignment = UITextAlignment.Right;
				cell.TextLabel.Font = UIFont.FromName (cell.TextLabel.Font.Name, 15);
				break;
			}

			return cell;
		}

		[Export ("tableView:numberOfRowsInSection:")]
		public nint RowsInSection (UITableView tableView, nint section)
		{
			return _receivedMessage.Count;
		}

		[Export ("textFieldShouldReturn:")]
		public virtual bool ShouldReturn (UITextField textField)
		{
			Console.WriteLine ("ShouldReturn");

			if (textField == nickName) {
				if (nickName.Text.Length > 0) {
					Socket socket = _io.GetSocket ();
			
//					socket.on("new message", std::bind(&OnNewMessage, (__bridge CFTypeRef)self, _1,_2,_3,_4));
//					socket.on("typing", std::bind(&OnTyping, (__bridge CFTypeRef)self, _1,_2,_3,_4));
//					socket.on("stop typing", std::bind(&OnStopTyping, (__bridge CFTypeRef)self, _1,_2,_3,_4));
//					socket.on("user joined", std::bind(&OnUserJoined, (__bridge CFTypeRef)self, _1,_2,_3,_4));
//					socket.on("user left", std::bind(&OnUserLeft, (__bridge CFTypeRef)self, _1,_2,_3,_4));
					socket.On ("login", (name, message, need_ack, ack_message) => {
						Console.WriteLine ("login");

//						if(data->get_flag() == message::flag_object)
//						{
//							NSInteger num = data->get_map()["numUsers"]->get_int();
						var num = 666;
						InvokeOnMainThread (() => {
							onLogin (num);
						});
//							dispatch_async(dispatch_get_main_queue(), ^{
//								[((__bridge CRViewController*)ctrl) onLogin:num];
//							});
//						}
					});
					_io.Connect ("http://chat.socket.io/");

					nickName.Enabled = false;
				}
			} else if (textField == messageField) {
				onSend (null);
			}

			return true;
		}

		//		-(void)updateTyping
		//		{
		//			NSString* typingMsg = nil;
		//			NSString* name = [_typingUsers anyObject];
		//			if (name) {
		//				if([_typingUsers count]>1)
		//				{
		//					typingMsg = [NSString stringWithFormat:@"%@ and %ld more are typing",name,[_typingUsers count]];
		//				}
		//				else
		//				{
		//					typingMsg =[NSString stringWithFormat:@"%@ is typing",name];
		//				}
		//			}
		//			self.typingLabel.text = typingMsg;
		//		}

		//		void OnNewMessage(CFTypeRef ctrl,string const& name,sio::message::ptr const& data,bool needACK,sio::message::ptr ackResp)
		//		{
		//			if(data->get_flag() == message::flag_object)
		//			{
		//				NSString* msg = [NSString stringWithUTF8String:data->get_map()["message"]->get_string().data()];
		//				NSString* user = [NSString stringWithUTF8String:data->get_map()["username"]->get_string().data()];
		//				dispatch_async(dispatch_get_main_queue(), ^{
		//					[((__bridge CRViewController*)ctrl) onNewMessage:msg from:user];
		//				});
		//			}
		//
		//		}

		//		void OnTyping(CFTypeRef ctrl,string const& name,sio::message::ptr const& data,bool needACK,sio::message::ptr ackResp)
		//		{
		//			if(data->get_flag() == message::flag_object)
		//			{
		//				NSString* user = [NSString stringWithUTF8String:data->get_map()["username"]->get_string().data()];
		//				dispatch_async(dispatch_get_main_queue(), ^{
		//					[((__bridge CRViewController*)ctrl) onUserTyping:user];
		//				});
		//			}
		//		}

		//		void OnStopTyping(CFTypeRef ctrl,string const& name,sio::message::ptr const& data,bool needACK,sio::message::ptr ackResp)
		//		{
		//			if(data->get_flag() == message::flag_object)
		//			{
		//				NSString* user = [NSString stringWithUTF8String:data->get_map()["username"]->get_string().data()];
		//				dispatch_async(dispatch_get_main_queue(), ^{
		//					[((__bridge CRViewController*)ctrl) onUserStopTyping:user];
		//				});
		//			}
		//		}

		//		void OnUserJoined(CFTypeRef ctrl, string const& name, sio::message::ptr const& data, bool needACK, sio::message::ptr ackResp)
		//		{
		//			if(data->get_flag() == message::flag_object)
		//			{
		//				NSString* user = [NSString stringWithUTF8String:data->get_map()["username"]->get_string().data()];
		//				NSInteger num = data->get_map()["numUsers"]->get_int();
		//				dispatch_async(dispatch_get_main_queue(), ^{
		//					[((__bridge CRViewController*)ctrl) onUserJoined:user participants:num];
		//				});
		//			}
		//		}

		//		void OnUserLeft(CFTypeRef ctrl, string const& name, sio::message::ptr const& data, bool needACK, sio::message::ptr ackResp)
		//		{
		//			if(data->get_flag() == message::flag_object)
		//			{
		//				NSString* user = [NSString stringWithUTF8String:data->get_map()["username"]->get_string().data()];
		//				NSInteger num = data->get_map()["numUsers"]->get_int();
		//				dispatch_async(dispatch_get_main_queue(), ^{
		//					[((__bridge CRViewController*)ctrl) onUserLeft:user participants:num];
		//				});
		//			}
		//		}
	}

	enum MessageFlag
	{
		Message_System,
		Message_Other,
		Message_You
	}

	class MessageItem
	{
		public string message { get; set; }

		public MessageFlag flag { get; set; }
	}
}

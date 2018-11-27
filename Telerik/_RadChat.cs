using sharpAHK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
//using System.Web;
//using System.Configuration;
using sharpAHK_Dev;
using System.Net.Mail;
using Telerik.WinControls.UI;
using System.Collections;
using Telerik.WinControls;
using TelerikExpressions;
using AHKExpressions;
using Telerik.WinControls.UI.Docking;
using System.Reflection;

namespace sharpAHK_Dev
{
    public partial class _TelerikLib
    {
        public class _RadChat
        {
            #region === Setup ===

            _AHK ahk = new _AHK();
            _Images img = new _Images();
            _Lists lst = new _Lists();

            // user icon locations defined in SetupRadChat()
            string adminImg2 = "";
            string adminImg3 = "";
            string adminImg4 = "";

            public static RadChat RadCHAT { get; set; }

            /// <summary>
            /// Configure RadChat Actions
            /// </summary>
            /// <param name="radchat"></param>
            public void SetupRadChat(RadChat radchat)
            {
                RadCHAT = radchat;
                RadCHAT.AutoAddUserMessages = false;
                RadCHAT.SendMessage += SendMessage;
                RadCHAT.TimeSeparatorAdding += TimeSeparatorAdding;
                RadCHAT.ItemFormatting += RadChat_ItemFormatting;


                adminImg2 = ahk.AppDir() + "\\ico\\admin2.ico";
                adminImg3 = ahk.AppDir() + "\\ico\\admin3.ico";
                adminImg4 = ahk.AppDir() + "\\ico\\admin4.ico";


                Author author2 = new Author(adminImg4.ToImage(), "Jason");
                RadCHAT.Author = author2;

                //RadCHAT.ItemFormatting += RadChat_ItemFormattingChildren;

                AddToolbar();
                //SetCustomFactory();
            }

            #endregion


            #region === Display Options ===

            /// <summary>
            /// Clears RadChat Conversation
            /// </summary>
            /// <param name="radchat"></param>
            public void Clear(RadChat radchat)
            {
                radchat.HideOverlay();
                radchat.ChatElement.SuggestedActionsElement.ClearActions();
                radchat.ChatElement.MessagesViewElement.Items.Clear();
            }

            /// <summary>
            /// Scroll to Bottom of RadChat Conversation
            /// </summary>
            /// <param name="radchat">RadChat Control</param>
            public void ScrollToBottom(RadChat radchat)
            {
                RadScrollBarElement scrollbar = radchat.ChatElement.MessagesViewElement.Scroller.Scrollbar;
                scrollbar.Value = 0;

                while (scrollbar.Value < scrollbar.Maximum - scrollbar.LargeChange + 1)
                {
                    scrollbar.PerformSmallIncrement(1);
                    radchat.ChatElement.MessagesViewElement.Scroller.UpdateScrollRange();
                    Application.DoEvents();
                }

                scrollbar.PerformLast();
            }

            /// <summary>
            /// Scroll to Top of RadChat Conversation
            /// </summary>
            /// <param name="radchat">RadChat Control</param>
            public void ScrollToTop(RadChat radchat)
            {
                RadScrollBarElement scrollbar = radchat.ChatElement.MessagesViewElement.Scroller.Scrollbar;
                scrollbar.Value = 0;

                while (scrollbar.Value<scrollbar.Maximum - scrollbar.LargeChange + 1)
                {
                    scrollbar.PerformSmallIncrement(-1);
                    radchat.ChatElement.MessagesViewElement.Scroller.UpdateScrollRange();
                    Application.DoEvents();
                }

                scrollbar.PerformLast();
            }

            public void ShowMessagesOnSide(RadChat radchat, bool OnSide = true)
            {
                radchat.ShowMessagesOnOneSide = OnSide;
            }
            public void ShowItemToolTips(RadChat radchat, bool ShowTips = true)
            {
                radchat.ShowItemToolTips = ShowTips;
            }
            public void ShowAvatars(RadChat radchat, bool ShowAvatars = true)
            {
                radchat.ShowAvatars = ShowAvatars;
            }


            #endregion


            #region === Add LIKE Icons to Messages ===


            public void CustomMessage(RadChat radchat)
            {
                radchat.ChatElement.ChatFactory = new MyChatFactory();
                radchat.Author = new Author(adminImg2.ToImage(), "Nancy");
                Author author2 = new Author(adminImg3.ToImage(), "Andrew");
                ChatTextMessage message1 = new ChatTextMessage("Hello", author2, DateTime.Now.AddHours(1));
                radchat.AddMessage(message1);
                ChatTextMessage message2 = new ChatTextMessage("Hi", radchat.Author, DateTime.Now.AddHours(1).AddMinutes(10));
                radchat.AddMessage(message2);
                ChatTextMessage message3 = new ChatTextMessage("We would like to announce that in the R2 2018 release we introduced Conversational UI", author2, DateTime.Now.AddHours(3));
                radchat.AddMessage(message3);
            }
            public class MyChatFactory : ChatFactory
            {
                public override BaseChatItemElement CreateItemElement(BaseChatDataItem item)
                {
                    if (item.GetType() == typeof(TextMessageDataItem))
                    {
                        return new MyTextMessageItemElement();
                    }
                    return base.CreateItemElement(item);
                }
            }
            public class MyTextMessageItemElement : TextMessageItemElement
            {
                LightVisualButtonElement likeButton = new LightVisualButtonElement();
                protected override void CreateChildElements()
                {
                    _AHK ahk = new _AHK();
                    _Images img = new _Images();
                    string HeartImage = ahk.AppDir() + "\\ico\\heart.png";
                    string HeartClickedImage = ahk.AppDir() + "\\ico\\heartClicked.png";

                    base.CreateChildElements();
                    likeButton.NotifyParentOnMouseInput = true;
                    likeButton.Image = HeartImage.ToImage(30, 30);
                    likeButton.Click += likeButton_Click;
                    likeButton.EnableElementShadow = false;
                    likeButton.Margin = new Padding(10, 0, 10, 0);
                    this.Children.Add(likeButton);
                }
                private void likeButton_Click(object sender, EventArgs e)
                {
                    if (this.Data.Tag == null)
                    {
                        this.Data.Tag = true;
                    }
                    else
                    {
                        bool isLiked = (bool)this.Data.Tag;
                        this.Data.Tag = !isLiked;
                    }
                }
                public override void Synchronize()
                {
                    _AHK ahk = new _AHK();
                    _Images img = new _Images();
                    string HeartImage = ahk.AppDir() + "\\ico\\heart.png";
                    string HeartClickedImage = ahk.AppDir() + "\\ico\\heartClicked.png";


                    base.Synchronize();
                    if (this.Data.Tag != null && (bool)this.Data.Tag == true)
                    {
                        this.likeButton.Image = HeartClickedImage.ToImage(30, 30);
                    }
                    else
                    {
                        this.likeButton.Image = HeartImage.ToImage(30, 30);
                    }
                }
                protected override SizeF ArrangeOverride(SizeF finalSize)
                {
                    SizeF baseSize = base.ArrangeOverride(finalSize);
                    RectangleF likeButtonRect;
                    RectangleF clientRect = this.GetClientRectangle(finalSize);
                    if (this.Data.ChatMessagesViewElement.ShowAvatars)
                    {
                        if (this.Data.ChatMessagesViewElement.ShowMessagesOnOneSide || !this.Data.IsOwnMessage)
                        {
                            likeButtonRect = new RectangleF(clientRect.X + this.AvatarPictureElement.DesiredSize.Width + this.MainMessageElement.DesiredSize.Width,
                                clientRect.Y + this.NameLabelElement.DesiredSize.Height + this.MainMessageElement.DesiredSize.Height / 3,
                                this.likeButton.Image.Width, this.likeButton.Image.Height);
                        }
                        else
                        {
                            likeButtonRect = new RectangleF(clientRect.Right - likeButton.DesiredSize.Width - this.AvatarPictureElement.DesiredSize.Width - this.MainMessageElement.DesiredSize.Width,
                                clientRect.Y + this.NameLabelElement.DesiredSize.Height + this.MainMessageElement.DesiredSize.Height / 3,
                                this.likeButton.Image.Width, this.likeButton.Image.Height);
                        }
                    }
                    else
                    {
                        if (this.Data.ChatMessagesViewElement.ShowMessagesOnOneSide || !this.Data.IsOwnMessage)
                        {
                            likeButtonRect = new RectangleF(clientRect.X + this.MainMessageElement.DesiredSize.Width,
                                clientRect.Y + this.NameLabelElement.DesiredSize.Height + this.MainMessageElement.DesiredSize.Height / 3,
                               this.likeButton.Image.Width, this.likeButton.Image.Height);
                        }
                        else
                        {
                            likeButtonRect = new RectangleF(clientRect.Right - likeButton.DesiredSize.Width - this.MainMessageElement.DesiredSize.Width,
                                clientRect.Y + this.NameLabelElement.DesiredSize.Height + this.MainMessageElement.DesiredSize.Height / 3,
                                this.likeButton.Image.Width, this.likeButton.Image.Height);
                        }
                    }
                    this.likeButton.Arrange(likeButtonRect);
                    return baseSize;
                }
            }


            #endregion


            #region === Message Formatting ===


            public void AddChildrenFormatting()
            {
                RadCHAT.ItemFormatting += RadChat_ItemFormattingChildren;
            }

            public void RemoveChildrenFormatting()
            {
                RadCHAT.ItemFormatting -= RadChat_ItemFormattingChildren;
            }


            Font f = new Font("Calibri", 12f, FontStyle.Bold);
            private void RadChat_ItemFormattingChildren(object sender, ChatItemElementEventArgs e)
            {
                ChatMessageAvatarElement avatar = e.ItemElement.AvatarPictureElement;
                ChatMessageNameElement name = e.ItemElement.NameLabelElement;
                ChatMessageStatusElement status = e.ItemElement.StatusLabelElement;
                LightVisualElement bubble = e.ItemElement.MainMessageElement;
                if (!e.ItemElement.IsOwnMessage && e.ItemElement is TextMessageItemElement)
                {
                    avatar.DrawImage = false;
                    name.Font = f;
                    bubble.DrawFill = true;
                    bubble.BackColor = Color.LightGreen;
                    bubble.ShadowDepth = 3;
                    bubble.ShadowColor = Color.Green;
                }
                else
                {
                    avatar.ResetValue(LightVisualElement.ImageProperty, Telerik.WinControls.ValueResetFlags.Local);
                    name.ResetValue(LightVisualElement.FontProperty, Telerik.WinControls.ValueResetFlags.All);
                    status.ResetValue(LightVisualElement.VisibilityProperty, Telerik.WinControls.ValueResetFlags.Local);
                    bubble.ResetValue(LightVisualElement.DrawFillProperty, Telerik.WinControls.ValueResetFlags.Local);
                    bubble.ResetValue(LightVisualElement.BackColorProperty, Telerik.WinControls.ValueResetFlags.Local);
                    bubble.ResetValue(LightVisualElement.ShadowDepthProperty, Telerik.WinControls.ValueResetFlags.Local);
                    bubble.ResetValue(LightVisualElement.ShadowColorProperty, Telerik.WinControls.ValueResetFlags.Local);
                }
            }



            /// <summary>
            /// Format Posted Messages based on message type (normal has one color, media message has another, carousel has another...
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void RadChat_ItemFormatting(object sender, ChatItemElementEventArgs e)
            {
                if (e.ItemElement is TextMessageItemElement)
                {
                    e.ItemElement.DrawBorder = true;
                    e.ItemElement.BorderBoxStyle = Telerik.WinControls.BorderBoxStyle.FourBorders;
                    e.ItemElement.BorderLeftColor = Color.Transparent;
                    e.ItemElement.BorderTopColor = Color.Transparent;
                    e.ItemElement.BorderRightColor = Color.Transparent;
                    e.ItemElement.BorderBottomColor = Color.LightBlue;
                }
                else if (e.ItemElement is MediaMessageItemElement)
                {
                    e.ItemElement.DrawFill = true;
                    e.ItemElement.BackColor = Color.LightGreen;
                }
                else if (e.ItemElement is CardMessageItemElement)
                {
                    e.ItemElement.DrawFill = true;
                    e.ItemElement.BackColor = Color.LightBlue;
                }
                else if (e.ItemElement is CarouselMessageItemElement)
                {
                    e.ItemElement.DrawFill = true;
                    e.ItemElement.BackColor = Color.LightCoral;
                }
                else
                {
                    e.ItemElement.ResetValue(LightVisualElement.DrawBorderProperty, Telerik.WinControls.ValueResetFlags.Local);
                    e.ItemElement.ResetValue(LightVisualElement.BorderBoxStyleProperty, Telerik.WinControls.ValueResetFlags.Local);
                    e.ItemElement.ResetValue(LightVisualElement.BorderLeftColorProperty, Telerik.WinControls.ValueResetFlags.Local);
                    e.ItemElement.ResetValue(LightVisualElement.BorderRightColorProperty, Telerik.WinControls.ValueResetFlags.Local);
                    e.ItemElement.ResetValue(LightVisualElement.BorderTopColorProperty, Telerik.WinControls.ValueResetFlags.Local);
                    e.ItemElement.ResetValue(LightVisualElement.BorderBottomColorProperty, Telerik.WinControls.ValueResetFlags.Local);
                    e.ItemElement.ResetValue(LightVisualElement.DrawFillProperty, Telerik.WinControls.ValueResetFlags.Local);
                    e.ItemElement.ResetValue(LightVisualElement.BackColorProperty, Telerik.WinControls.ValueResetFlags.Local);
                }
            }


            #endregion


            #region === Custom ChatFactory ===

            /// <summary>
            /// Unclear what this can do 
            /// </summary>
            public void SetCustomFactory()
            {
                RadCHAT.ChatElement.ChatFactory = new CustomChatFactory();
            }
            public class CustomChatFactory : ChatFactory
            {
                public override BaseChatItemElement CreateItemElement(BaseChatDataItem item)
                {
                    if (item.GetType() == typeof(TextMessageDataItem))
                    {
                        return new TextMessageItemElement();
                    }
                    else if (item.GetType() == typeof(CardMessageDataItem))
                    {
                        return new CardMessageItemElement();
                    }
                    else if (item.GetType() == typeof(CarouselMessageDataItem))
                    {
                        return new CarouselMessageItemElement();
                    }
                    else if (item.GetType() == typeof(MediaMessageDataItem))
                    {
                        return new MediaMessageItemElement();
                    }
                    else if (item.GetType() == typeof(ChatTimeSeparatorDataItem))
                    {
                        return new ChatTimeSeparatorItemElement();
                    }
                    return base.CreateItemElement(item);
                }
                public override BaseChatCardElement CreateCardElement(BaseChatCardDataItem cardDataItem)
                {
                    if (cardDataItem.GetType() == typeof(ChatFlightCardDataItem))
                    {
                        return new ChatFlightCardElement(cardDataItem as ChatFlightCardDataItem);
                    }
                    else if (cardDataItem.GetType() == typeof(ChatImageCardDataItem))
                    {
                        return new ChatImageCardElement(cardDataItem as ChatImageCardDataItem);
                    }
                    else if (cardDataItem.GetType() == typeof(ChatProductCardDataItem))
                    {
                        return new ChatProductCardElement(cardDataItem as ChatProductCardDataItem);
                    }
                    else if (cardDataItem.GetType() == typeof(ChatWeatherCardDataItem))
                    {
                        return new ChatWeatherCardElement(cardDataItem as ChatWeatherCardDataItem);
                    }
                    return base.CreateCardElement(cardDataItem);
                }
                public override ToolbarActionElement CreateToolbarActionElement(ToolbarActionDataItem item)
                {
                    return new ToolbarActionElement(item);
                }
                public override SuggestedActionElement CreateSuggestedActionElement(SuggestedActionDataItem item)
                {
                    return new SuggestedActionElement(item);
                }
                public override BaseChatDataItem CreateDataItem(ChatMessage message)
                {
                    ChatTextMessage textMessage = message as ChatTextMessage;
                    if (textMessage != null)
                    {
                        return new TextMessageDataItem(textMessage);
                    }
                    ChatMediaMessage mediaMessage = message as ChatMediaMessage;
                    if (mediaMessage != null)
                    {
                        return new MediaMessageDataItem(mediaMessage);
                    }
                    ChatCardMessage cardMessage = message as ChatCardMessage;
                    if (cardMessage != null)
                    {
                        return new CardMessageDataItem(cardMessage);
                    }
                    ChatCarouselMessage carouselMessage = message as ChatCarouselMessage;
                    if (carouselMessage != null)
                    {
                        return new CarouselMessageDataItem(carouselMessage);
                    }
                    return base.CreateDataItem(message);
                }
            }


            #endregion


            #region === Toolbar ===

            public void AddToolbar()
            {
                string FileImg = ahk.AppDir() + "\\ico\\File.png";

                ToolbarActionDataItem imageAction = new ToolbarActionDataItem(FileImg.ToImage(30, 30), "image");
                RadCHAT.ChatElement.ToolbarElement.AddToolbarAction(imageAction);
                RadCHAT.ToolbarActionClicked += radChat_ToolbarActionClicked;
            }

            public void radChat_ToolbarActionClicked(object sender, ToolbarActionEventArgs e)
            {
                ToolbarActionDataItem action = e.DataItem;
                if (action.UserData + "" == "image")
                {
                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.Title = "Open Image";
                    dlg.Filter = "png files (*.png)|*.png";
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        Image img = Image.FromFile(dlg.FileName);
                        ChatMediaMessage mediaMessage = new ChatMediaMessage(img, new Size(300, 200), null, RadCHAT.Author, DateTime.Now);
                        RadCHAT.AddMessage(mediaMessage);
                    }
                    dlg.Dispose();
                }
            }


            #endregion


            #region === Chat Actions / Suggestions ===

            // Chat Suggestions

            public void AddSuggestedActions(RadChat radchat)
            {
                radchat.AddMessage(new ChatTextMessage("Hello, here are the choices", radchat.Author, DateTime.Now));

                List<SuggestedActionDataItem> actions = new List<SuggestedActionDataItem>();
                for (int i = 0; i < 7; i++)
                {
                    actions.Add(new SuggestedActionDataItem("Option " + (i + 1)));
                }
                Author author = new Author(adminImg3.ToImage(), "Andrew");
                ChatSuggestedActionsMessage suggestionActionsMessage = new ChatSuggestedActionsMessage(actions, author, DateTime.Now);
                radchat.AddMessage(suggestionActionsMessage);
                radchat.SuggestedActionClicked += radChat_SuggestedActionClicked;
            }

            public void radChat_SuggestedActionClicked(object sender, SuggestedActionEventArgs e)
            {
                RadCHAT.AddMessage(new ChatTextMessage("You have chosen " + e.Action.Text, RadCHAT.Author, DateTime.Now));
            }


            #endregion


            #region === Send Messages ===

            /// <summary>
            /// Modify User's Send Text Before Posting
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public void SendMessage(object sender, SendMessageEventArgs e)
            {
                ChatTextMessage textMessage = e.Message as ChatTextMessage;
                //textMessage.Message = "[Slightly changed message] " + textMessage.Message;  // add prefix to user message on sending
                textMessage.Message = textMessage.Message;  // no alteration to user message on sending
                RadCHAT.AddMessage(textMessage);
            }

            /// <summary>
            /// Define the idle time before adding timestamp to conversation 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void TimeSeparatorAdding(object sender, TimeSeparatorEventArgs e)
            {
                if (e.Item != null && e.PreviousItem != null)
                {
                    e.ShouldAddSeparator = e.Item.Message.TimeStamp - e.PreviousItem.Message.TimeStamp > new TimeSpan(0, 0, 20);
                }
            }

            /// <summary>
            /// example of adding message with image
            /// </summary>
            /// <param name="radchat"></param>
            public void mediaMsg(RadChat radchat)
            {
                Author author2 = new Author(adminImg4.ToImage(), "Nancy");

                RadCHAT.Author = author2;

                

                string Image = @"C:\Users\jason\OneDrive\Pictures\MEMES\Memes\IMG_5202.JPG";

                ChatMediaMessage mediaMessage = new ChatMediaMessage(Image.ToImage(), new Size(300, 600), null, RadCHAT.Author, DateTime.Now);
                radchat.AddMessage(mediaMessage);
            }

            /// <summary>
            /// Example of adding messages from  this user and other user
            /// </summary>
            /// <param name="radchat"></param>
            public void addmessages(RadChat radchat)
            {
                Author author2 = new Author(adminImg2.ToImage(), "Nancy");
                Author author3 = new Author(adminImg3.ToImage(), "Andrew");

                radchat.Author = author3;  // assigns who the "This Sender" author is 

                ChatTextMessage message1 = new ChatTextMessage("Hello", author2, DateTime.Now.AddHours(1));
                radchat.AddMessage(message1);

                ChatTextMessage message2 = new ChatTextMessage("Hi", author3, DateTime.Now.AddHours(1).AddMinutes(10));
                radchat.AddMessage(message2);

                ChatTextMessage message3 = new ChatTextMessage("How are you?", author2, DateTime.Now.AddHours(3));
                radchat.AddMessage(message3);
            }


            #endregion


            #region === Chat Overlays ===

            // Chat Overlays 

            public void ChatOverlay_List(RadChat radchat)
            {
                ChatListOverlay chatListOverlay = new ChatListOverlay("List overlay");
                for (int i = 0; i < 10; i++)
                {
                    chatListOverlay.ListView.Items.Add("Item " + i);
                }
                bool showAsPopup = false;
                Author author = new Author(adminImg3.ToImage(), "Andrew");
                ChatOverlayMessage overlayMessage = new ChatOverlayMessage(chatListOverlay, showAsPopup, author, DateTime.Now);
                radchat.AddMessage(overlayMessage);
            }

            public void ChatOverlay_Calendar(RadChat radchat)
            {
                ChatCalendarOverlay chatListOverlay = new ChatCalendarOverlay("Calendar Overlay");

                bool showAsPopup = false;
                Author author = new Author(adminImg3.ToImage(), "Andrew");
                ChatOverlayMessage overlayMessage = new ChatOverlayMessage(chatListOverlay, showAsPopup, author, DateTime.Now);
                radchat.AddMessage(overlayMessage);
            }

            public void ChatOverlay_Base(RadChat radchat)
            {
                BaseChatOverlay chatListOverlay = new BaseChatOverlay();

                bool showAsPopup = false;
                Author author = new Author(adminImg3.ToImage(), "Andrew");
                ChatOverlayMessage overlayMessage = new ChatOverlayMessage(chatListOverlay, showAsPopup, author, DateTime.Now);
                radchat.AddMessage(overlayMessage);
            }

            public void ChatOverlay_Time(RadChat radchat)
            {
                ChatTimeOverlay chatListOverlay = new ChatTimeOverlay("Time Over", DateTime.Now);

                bool showAsPopup = false;
                Author author = new Author(adminImg3.ToImage(), "Andrew");
                ChatOverlayMessage overlayMessage = new ChatOverlayMessage(chatListOverlay, showAsPopup, author, DateTime.Now);
                radchat.AddMessage(overlayMessage);
            }

            public void ChatOverlay_DateTime(RadChat radchat)
            {
                ChatDateTimeOverlay dateTimerOverlay = new ChatDateTimeOverlay("Select a date and time", DateTime.Now);
                bool showAsPopup = false;
                Author author = new Author(adminImg3.ToImage(), "Andrew");
                ChatOverlayMessage overlayMessage = new ChatOverlayMessage(dateTimerOverlay, showAsPopup, author, DateTime.Now);
                radchat.AddMessage(overlayMessage);
            }


            // Custom Control Overlays -- RadTreeView

            public void ChatOverlay_RadTreeView()
            {
                CustomBaseChatItemOverlay customOverlay = new CustomBaseChatItemOverlay("Custom overlay");

                //DataTable dt = new DataTable();
                //dt.Columns.Add("Id", typeof(int));
                //dt.Columns.Add("Name", typeof(string));
                //for (int i = 0; i < 10; i++)
                //{
                //    dt.Rows.Add(i, "Item" + i);
                //}
                //customOverlay.Mccb.DisplayMember = "Name";
                //customOverlay.Mccb.ValueMember = "Id";
                //customOverlay.Mccb.DataSource = dt;

                RadTreeNode node = new RadTreeNode();
                node.Text = "this text";
                customOverlay.Mccb.Nodes.Add(node);


                bool showAsPopup = false;
                Author author = new Author(adminImg2.ToImage(), "Andrew");
                RadCHAT.Author = author;
                ChatOverlayMessage overlayMessage = new ChatOverlayMessage(customOverlay, showAsPopup, author, DateTime.Now);
                RadCHAT.AddMessage(overlayMessage);
            }

            public class CustomBaseChatItemOverlay : BaseChatItemOverlay
            {
                public CustomBaseChatItemOverlay(string title)
                    : base(title)
                {
                    mccb.SelectedNodeChanged += radTreeView_SelectedNodeChanged;
                }
                public RadTreeView Mccb
                {
                    get
                    {
                        return this.mccb;
                    }
                }
                private void radTreeView_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
                {
                    //_AHK ahk = new _AHK();
                    //ahk.MsgBox("node changed");
                    if (e.Node != null)
                    {
                        this.CurrentValue = e.Node.Text;
                    }
                }
                RadTreeView mccb;
                protected override Telerik.WinControls.RadElement CreateMainElement()
                {
                    mccb = new RadTreeView();
                    return new Telerik.WinControls.RadHostItem(this.mccb);
                }
                protected override void DisposeManagedResources()
                {
                    mccb.SelectedNodeChanged -= radTreeView_SelectedNodeChanged;
                    base.DisposeManagedResources();
                }
            }


            #endregion


            #region === Cards / Carousel ===

            string Image1 = @"C:\Users\jason\OneDrive\Pictures\MEMES\Memes\IMG_5078.PNG";
            string Image2 = @"C:\Users\jason\OneDrive\Pictures\MEMES\Memes\IMG_5560.JPG";
            string Image3 = @"C:\Users\jason\OneDrive\Pictures\MEMES\Memes\IMG_8163.JPG";
            string Image4 = @"C:\Users\jason\OneDrive\Pictures\MEMES\Memes\IMG_5390.JPG";

            // Chat Cards / Carousel

            public void Add_CarouselMessage(RadChat radchat)
            {



                Telerik.WinControls.UI.ChatImageCardDataItem imageCard = new ChatImageCardDataItem(Image1.ToImage(),
                    "Benjamin Vilanders", "Senior Architect",
                    "As a Senior Architect his experience in the industry allows him to take on increased responsibility. Like other architects, he design buildings " +
                    "and makes sure they are structurally sound. Due to his track record of quality performance, Benjamin also serves as a manager, a mentor, an advisor and coordinator.",
                    null, null);

                ChatProductCardDataItem productCard = new ChatProductCardDataItem(Image2.ToImage(),
                    "Arrive & Drive", "Rating 7/10",
                    "With our Arrive & Drive Packages, the only thing you will have to think about is driving. We make it simple for you to get more of what you love. We streamline the " +
                    "entire process and have everything ready for you when you arrive at the track so you can get straight to racing.", "Packages from $340",
                    null, null);

                ChatWeatherCardDataItem weatherCard = new ChatWeatherCardDataItem("Florence", Image3.ToImage(),
                    "33°C", "Humidity: 76%", "Dew: 31°C",
                    "Pressure: 1031 mb", "Wind Speed: 15 km/h NW");

                List<FlightInfo> flights = new List<FlightInfo>();
                flights.Add(new FlightInfo("Los Angelis", "LAX", DateTime.Now.AddDays(7), "New York", "JFK", DateTime.Now.AddDays(7).AddHours(5.5)));
                flights.Add(new FlightInfo("New York", "JFK", DateTime.Now.AddDays(14).AddHours(3), "Los Angelis", "LAX", DateTime.Now.AddDays(14).AddHours(9.1)));
                ChatFlightCardDataItem flightCard = new ChatFlightCardDataItem("Andrew Fuller", flights, Image4.ToImage(), "$341", null);

                List<BaseChatCardDataItem> cards = new List<BaseChatCardDataItem>();
                cards.Add(imageCard);
                cards.Add(productCard);
                cards.Add(weatherCard);
                cards.Add(flightCard);
                Author author = new Author(Image3.ToImage(), "Ben");

                ChatCarouselMessage carouselMessage = new ChatCarouselMessage(cards, author, DateTime.Now);
                radchat.AddMessage(carouselMessage);
            }

            /// <summary>
            /// Add a ChatImageCardElement programmatically
            /// </summary>
            /// <param name="radchat">RadChat Control To Populate</param>
            public void ChatImageCardElement(RadChat radchat)
            {
                Telerik.WinControls.UI.ChatImageCardDataItem imageCard = new ChatImageCardDataItem(Image1.ToImage(), "Benjamin Vilanders", "Senior Architect",
                "As a Senior Architect his experience in the industry allows him to take on increased responsibility. Like other architects, he design buildings " +
                "and makes sure they are structurally sound. Due to his track record of quality performance, Benjamin also serves as a manager, a mentor, an advisor and coordinator.",
                null, null);

                Author author = new Author(adminImg2.ToImage(), "Ben");
                ChatCardMessage message = new ChatCardMessage(imageCard, author, DateTime.Now);
                radchat.AddMessage(message);
            }

            public void ChatProductCardElement(RadChat radchat)
            {
                ChatProductCardDataItem productCard = new ChatProductCardDataItem(Image2.ToImage(), "Arrive & Drive", "Rating 7/10",
                "With our Arrive & Drive Packages, the only thing you will have to think about is driving. We make it simple for you to get more of what you love. We streamline the " +
                "entire process and have everything ready for you when you arrive at the track so you can get straight to racing.", "Packages from $340", null, null);

                Author author = new Author(adminImg2.ToImage(), "Ben");
                ChatCardMessage message = new ChatCardMessage(productCard, author, DateTime.Now);
                radchat.AddMessage(message);
            }

            public void ChatWeatherCardElement(RadChat radchat)
            {
                ChatWeatherCardDataItem weatherCard = new ChatWeatherCardDataItem("Florence", Image3.ToImage(), "33°C", "Humidity: 76%", "Dew: 31°C",
                    "Pressure: 1031 mb", "Wind Speed: 15 km/h NW");

                Author author = new Author(adminImg3.ToImage(), "Nancy");
                ChatCardMessage message = new ChatCardMessage(weatherCard, author, DateTime.Now);
                radchat.AddMessage(message);
            }

            public void ChatFlightCardElement(RadChat radchat)
            {
                List<FlightInfo> flights = new List<FlightInfo>();
                flights.Add(new FlightInfo("Los Angelis", "LAX", DateTime.Now.AddDays(7), "New York", "JFK", DateTime.Now.AddDays(7).AddHours(5.5)));
                flights.Add(new FlightInfo("New York", "JFK", DateTime.Now.AddDays(14).AddHours(3), "Los Angelis", "LAX", DateTime.Now.AddDays(14).AddHours(9.1)));
                ChatFlightCardDataItem flightCard = new ChatFlightCardDataItem("Andrew Fuller", flights, Image4.ToImage(), "$341", null);

                Author author = new Author(adminImg3.ToImage(), "Nancy");
                ChatCardMessage message = new ChatCardMessage(flightCard, author, DateTime.Now);
                radchat.AddMessage(message);
            }


            #endregion


        }

    }
}


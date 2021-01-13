AuchoDB={};
ItemDB = {"Volatile Fire", "Volatile Earth"};
thispage=0;
total=0;
thisitem=1;
fire=0;
CanISendQuery=nil;
timestamp=0;
MaxAuctions=0;
EndAuctions=0;
busy=0;
auctions_without_this_page=0;
auctions_with_this_page=0;

local frame = CreateFrame("FRAME", "AuchoEventFrame");


frame:RegisterEvent("AUCTION_ITEM_LIST_UPDATE");
local function eventHandler(self, event, ...)
if event=="AUCTION_ITEM_LIST_UPDATE" then
	ReadTheAuctionHouse();
      end
end 
                                       
local function updateHandler(self, elapsed)
	total=total+elapsed;
	if total>2 then
	    	if fire==1 then
			if CanSendAuctionQuery() then
				fire = 0;
				
				QueryAuctionItems(ItemDB[thisitem], 0, 0, 0, 0, 0, thispage, 0, 0, 0); -- get next page
                                busy=0;
			
        	        end
	        end
		total = 0;
	end
end

frame:SetScript("OnEvent", eventHandler);
frame:SetScript("OnUpdate", updateHandler);

function Scan()
	busy=0;
	thispage=0;
	thisitem=1;
	CanISendQuery=nil
	timestamp=time()-1299190000;
	auctions_without_this_page=0;
	auctions_with_this_page=0;
	frame:RegisterEvent("AUCTION_ITEM_LIST_UPDATE");
	fire=1;
end

local mainWnd = CreateFrame("button", "Aucho", UIParent, "UIPanelButtonTemplate");
mainWnd:SetWidth(100);
mainWnd:SetHeight(20);
mainWnd:SetPoint("CENTER",100);
mainWnd:SetText("Go");
mainWnd:SetScript("OnClick", Scan);

function ReadTheAuctionHouse()
 	if busy==0  then
busy=1;
		_,MaxAuctions = GetNumAuctionItems("list");

		auctions_without_this_page=thispage*50;
		auctions_with_this_page=auctions_without_this_page + 50;
		if auctions_with_this_page>MaxAuctions then
			EndAuctions= MaxAuctions - thispage*50;
	   
		else
			EndAuctions = 50;
		end

		for i = 1,EndAuctions do
		      local name, texture, count, quality, canUse, level, minBid, minIncrement, buyoutPrice, bidAmount, highBidder, owner;
		      name, texture, count, quality, canUse, level, minBid, minIncrement, buyoutPrice, bidAmount, highBidder, owner = GetAuctionItemInfo("list", i);
		      AuchoDB[#AuchoDB+1] = ItemDB[thisitem] .. ":" .. count .. ":" .. buyoutPrice .. ":" .. timestamp;
		end

                if EndAuctions<50 then
			thispage=0;
			if thisitem<#ItemDB then
			        thisitem = thisitem+1;
				fire=1;
			else
				frame:UnregisterEvent("AUCTION_ITEM_LIST_UPDATE"); 
			end
		else
			thispage = thispage + 1;
			fire=1;
		end
		
	end
end
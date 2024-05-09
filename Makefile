# **************************************************************************** #
#                                                                              #
#                                                         :::      ::::::::    #
#    Makefile                                            :+:      :+:    :+:    #
#                                                     +:+ +:+         +:+      #
#    By: tunsinge <tunsinge@42mulhouse.fr           +#+  +:+       +#+         #
#                                                 +#+#+#+#+#+   +#+            #
#    Created: 2024/05/09 18:30:00 by tunsinge          #+#    #+#              #
#    Updated: 2024/05/09 18:30:00 by tunsinge         ###   ########.fr        #
#                                                                              #
# **************************************************************************** #

#//= Colors =//#
BOLD			:= \033[1m
BLACK			:= \033[30;1m
RED				:= \033[31;1m
GREEN			:= \033[32;1m
YELLOW			:= \033[33;1m
BLUE			:= \033[34;1m
MAGENTA			:= \033[35;1m
CYAN			:= \033[36;1m
WHITE			:= \033[37;1m
RESET			:= \033[0m

#//= Variables =//#
NAME			:= ft_turing
PROJECT_NAME	:= FT_TURING
DIRECTORY		:= ft_turing

#//= Main compiling rules =//#

all: startall $(NAME) endall

$(NAME) :
	dotnet publish -o $(DIRECTORY) -c=Release

example:
	./$(DIRECTORY)/ft_turing machine2.json "11+1="

#//= Cleaning rules =//#

clean:
	@printf "$(YELLOW)--------------------\n$(RESET)"
	@rm -rf $(DIRECTORY) obj bin && printf "$(GREEN)Cleaning $(PROJECT_NAME) OBJS...\n$(RESET)"
	@printf "$(YELLOW)Finished cleaning !\n$(RESET)"
	@printf "$(YELLOW)--------------------\n$(RESET)"

#//= Beautification rules =//#

startall:
	@printf "$(BOLD)--------------------\n$(RESET)"
	@printf "$(MAGENTA)Starting doing everything\n$(RESET)"
	@printf "$(BOLD)--------------------\n$(RESET)"

endall:
	@printf "$(BOLD)--------------------\n$(RESET)"
	@printf "$(BOLD)Done compiling $(MAGENTA)e$(CYAN)v$(GREEN)e$(YELLOW)r$(BLUE)y$(RED)t$(MAGENTA)h$(CYAN)i$(GREEN)n$(YELLOW)g !\n$(RESET)$(BOLD)--------------------\n$(RESET)$(MAGENTA)Done making $(PROJECT_NAME)!\n$(RESET)";

re:	clean all

.PHONY: all clean re startall endall